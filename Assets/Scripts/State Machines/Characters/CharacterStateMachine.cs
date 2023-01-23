using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Zenject;
using Animations;
using Units;
using Pathfinding;
using Units.Attributes;
using System.Collections;
using UnityEngine.UI;
using TMPro;

namespace States.Characters
{
    public abstract class CharacterStateMachine : BaseStateMachine
    {
        [SerializeField] private UnitConfig _config;
        [Space]
        //[SerializeField] protected NavMeshAgent _agent;
        //public NavMeshAgent Agent => _agent;
        [SerializeField] private AIPath _aiPath;
        public AIPath AIPath => _aiPath;
        [SerializeField] protected UnitHealth _health;
        public UnitHealth Health => _health;
        [SerializeField] protected CharacterAnimationController _animationController;
        public CharacterAnimationController AnimationController => _animationController;
        [SerializeField] private Collider _collider;
        [SerializeField] private Rigidbody _rigidBody;
        [field: SerializeField] public Seeker Seeker { get; private set; }

        [Header("Combat Properties")]
        [SerializeField] private Image _range;
        [SerializeField] private Image _melee;
        [SerializeField] public TMP_Text _stateText;
        [SerializeField] private SphereCollider _agroTrigger;
        [SerializeField] private LayerMask _targetMask;
        [SerializeField] private LayerMask _obstaclesMask;
        [field: SerializeField] public Transform RangeAttackProjectilePoint { get; private set; }
        [field: SerializeField] public bool DontAttackUntilReachedDestination { get; private set; } = true;
        [SerializeField] private GameObject _deadParticle;

        private Coroutine _chargingAttackCor;

        private float _moveSpeedModificator = 1f;
        private List<UnitHealth> _targets = new List<UnitHealth>();
        public IList<UnitHealth> Targets => _targets.AsReadOnly();
        [HideInInspector] public UnitHealth Target = null;
        public AttackType CurrentAttackType { get; private set; }
        public bool MeleeAttackCharged { get; private set; }
        public bool RangeAttackCharged { get; private set; }
        public bool CurrentAttackCharged => CurrentAttackType == AttackType.Melee ? MeleeAttackCharged : RangeAttackCharged;

        public AttackAttributes CurrentAttack
        {
            get
            {
                if (CurrentAttackType == AttackType.Range)
                    return Combat.Range;
                else
                    return Combat.Melee;
            }
        }
        [HideInInspector] public int AccuracyPseudoRandomMultiplier;

        public UnitCombatAttributes Combat => _config.Combat;
        public UnitMovementAttributes Movement => _config.Movement;

        public UnitsGroup UnitsGroup { get; private set; }
        public Vector3 HeldedPosition { get; private set; }
        [HideInInspector] public Vector3 BattleHeldedPosition;

        protected new CharacterStateFactory States { get; private set; }

        public Action OnHeldedPositionSetted { get; set; }
        public Action OnAttackTypeSwitched { get; set; }
        public Action<CharacterStateMachine, AttackType> OnAttackTypeGroupSwitched { get; set; }
        public Action<CharacterStateMachine> OnFindTarget { get; set; }
        public Action<CharacterStateMachine> OnLostAllTargets { get; set; }
        public Action<CharacterStateMachine> OnDead { get; set; }

        protected override void Start()
        {
            base.Start();

            SetMoveSpeed(_config.Movement.MoveSpeed);
            _aiPath.rotationSpeed = _config.Movement.RotationSpeed;

            _health.Init(_config.Health);

            switch (_config.Combat.AttackType)
            {
                case AttackType.Both:
                case AttackType.Range:
                    SwitchAttackType(AttackType.Range);
                    break;
                case AttackType.Melee:
                    SwitchAttackType(AttackType.Melee);
                    break;
            }
        }

        protected override void Update()
        {
            base.Update();

            _range.color = RangeAttackCharged ? Color.green : Color.red;
            _melee.color = MeleeAttackCharged ? Color.green : Color.red;
            //_stateText.text = CurrentAttackType.ToString();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (UnitHealth target in _targets)
                target.OnDead -= RemoveTargetFromList;
        }

        protected override void InitializeState()
        {
            States = _factory.Create<CharacterStateFactory>(this);
            CurrentState = States.Neutral();
            CurrentState.Enter();
        }

        public virtual void SetUnitsGroup(UnitsGroup unitsGroup)
        {
            UnitsGroup = unitsGroup;
        }

        public virtual void Dead()
        {
            _rigidBody.isKinematic = true;
            _collider.enabled = false;
            _aiPath.enabled = false;
            OnDead?.Invoke(this);

            Instantiate(_deadParticle, transform.position + Vector3.up, transform.rotation);

            Destroy(gameObject, 5f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((1 << other.gameObject.layer & _targetMask) != 0)
            {
                if (other.TryGetComponent(out UnitHealth health) == true)
                {
                    AddTargetToList(health);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if ((1 << other.gameObject.layer & _targetMask) != 0)
            {
                if (other.TryGetComponent(out UnitHealth health) == true)
                {
                    RemoveTargetFromList(health);
                }
            }
        }

        public void AddTargetsToList(UnitHealth[] targets)
        {
            foreach (UnitHealth target in targets)
                AddTargetToList(target);
        }

        private void AddTargetToList(UnitHealth target)
        {
            if (_targets.Contains(target) == true || target.IsDead == true) return;

            target.OnDead += RemoveTargetFromList;
            _targets.Add(target);

            if (_targets.Count == 1)
                OnFindTarget?.Invoke(this);
        }

        private void RemoveTargetFromList(UnitHealth target)
        {
            if (_targets.Contains(target) == true)
            {
                target.OnDead -= RemoveTargetFromList;
                _targets.Remove(target);

                if (_targets.Count <= 0)
                {
                    Target = null;
                    OnLostAllTargets?.Invoke(this);
                }
            }
        }

        public UnitHealth GetClosestTarget(UnitHealth[] targets = null, bool ignoreObstacles = false)
        {
            if (targets == null)
                targets = _targets.ToArray();

            UnitHealth closestTarget = null;
            float minDistance = float.MaxValue;
            bool onlyFreeTargets = true;

            for (int i = 0; i < 2; i++)
            {
                foreach (UnitHealth target in targets)
                {
                    if (target.IsDead == true) continue;

                    if (ignoreObstacles == false)
                    {
                        Vector3 dir = target.transform.position - transform.position;

                        if (Physics.Raycast(transform.position + Vector3.up, dir.normalized, dir.magnitude, _obstaclesMask) == true)
                            continue;
                    }

                    if (onlyFreeTargets == true && target.IsTargeted == true)
                    {
                        if (target.CountOfTargeted == 1)
                        {
                            if (target.IsMyTarget(this) == false)
                                continue;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    float distance = Vector3.Distance(transform.position, target.transform.position);
                
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestTarget = target;
                    }
                }

                onlyFreeTargets = false;

                if (closestTarget != null) break;
            }

            return closestTarget;
        }

        public void SetDestination(Vector3 destination, bool heldedPosition = false)
        {
            _aiPath.destination = destination;

            if (heldedPosition == true)
            {
                HeldedPosition = destination;
                OnHeldedPositionSetted?.Invoke();
            }
        }

        public void SwitchAttackType(AttackType newAttackType, bool groupCallback = true)
        {
            if (newAttackType == CurrentAttackType) return;

            if(newAttackType == AttackType.Melee)
                ResetAttackPoint(AttackType.Range);

            CurrentAttackType = newAttackType;
            _agroTrigger.radius = CurrentAttack.AgroRadius;
            OnAttackTypeSwitched?.Invoke();

            if (groupCallback == true)
                OnAttackTypeGroupSwitched?.Invoke(this, CurrentAttackType);
        }

        public void ChargeAttackPoint(AttackType attackType, float time)
        {
            if (attackType == AttackType.Melee)
            {
                if (MeleeAttackCharged == true) return;
            }
            else
            {
                if (RangeAttackCharged == true) return;
            }

            if (_chargingAttackCor != null)
                StopCoroutine(_chargingAttackCor);

            _chargingAttackCor = StartCoroutine(ChargingAttackPoint(attackType, time));
        }

        private IEnumerator ChargingAttackPoint(AttackType attackType, float time)
        {
            yield return new WaitForSeconds(time);

            if (attackType == AttackType.Melee)
            {
                MeleeAttackCharged = true;
                RangeAttackCharged = false;
            }
            else
            {
                RangeAttackCharged = true;
                MeleeAttackCharged = false;
            }

            _chargingAttackCor = null;
        }

        public void ResetAttackPoint(AttackType attackType)
        {
            if (_chargingAttackCor != null)
            {
                StopCoroutine(_chargingAttackCor);
                _chargingAttackCor = null;
            }

            if(attackType == AttackType.Melee)
                MeleeAttackCharged = false;
            else
                RangeAttackCharged = false;
        }

        public void SetMoveSpeed(float moveSpeed)
        {
            _aiPath.maxSpeed = moveSpeed * _moveSpeedModificator;
        }

        public void SetMoveSpeedMofidicator(float modificator, float duration)
        {
            _moveSpeedModificator = modificator;
            CancelInvoke();
            Invoke(nameof(ResetSpeedModificator), duration);
        }

        private void ResetSpeedModificator()
        {
            _moveSpeedModificator = 1f;
        }

        public bool IsHaveTargetInSight()
        {
            if (Targets.Count == 0) return false;

            foreach (UnitHealth target in Targets)
            {
                Vector3 dir = (target.transform.position - transform.position);

                if (Physics.Raycast(transform.position + Vector3.up, dir.normalized, dir.magnitude, _obstaclesMask) == true)
                    continue;

                return true;
            }

            return false;
        }

        public class Factory : PrefabFactory<CharacterStateMachine> { }
    }
}