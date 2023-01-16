﻿using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Zenject;
using Animations;
using Units;
using Pathfinding;
using Units.Attributes;
using System.Collections;

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

        [Header("Combat Properties")]
        [SerializeField] private SphereCollider _agroTrigger;
        //[SerializeField] private float _agroRadius = 5f;
        [SerializeField] private LayerMask _targetMask;
        //[SerializeField] private bool _ignoreTargetsWhenMove;
        //public bool IgnoreTargetsWhenMove => _ignoreTargetsWhenMove;
        //[SerializeField] private float _stopIgnoringDestinationDistance = 2f;
        //public float StopIgnoringDestinationDistance => _stopIgnoringDestinationDistance;


        //[Header("Attack Properties")]
        //[SerializeField] private float _attackRadius = 1f;
        //public float AttackRadius => _attackRadius;
        //[SerializeField] private int _damage;
        //public int Damage => _damage;
        //[SerializeField] private float _attackInterval = 1f;
        //public float AttackInterval => _attackInterval;
        //[SerializeField, Range(0f, 1f)] private float _accuracy = .5f;
        //public float Accuracy => _accuracy;
        private Coroutine _chargingAttackCor;

        private List<UnitHealth> _targets = new List<UnitHealth>();
        public IList<UnitHealth> Targets => _targets.AsReadOnly();
        public UnitHealth Target;
        public AttackType CurrentAttackType { get; private set; }
        public bool AttackCharged { get; private set; }

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

        public UnitCombatAttributes Combat => _config.Combat;

        public Vector3 HeldedPosition { get; private set; }

        protected new CharacterStateFactory States { get; private set; }

        public Action OnHeldedPositionSetted { get; set; }
        public Action<CharacterStateMachine, AttackType> OnAttackTypeSwitched { get; set; }
        public Action<CharacterStateMachine> OnFindTarget { get; set; }
        public Action<CharacterStateMachine> OnLostAllTargets { get; set; }
        public Action<CharacterStateMachine> OnDead { get; set; }

        protected override void Start()
        {
            base.Start();

            _aiPath.maxSpeed = _config.Movement.MoveSpeed;
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

        }

        public virtual void Dead()
        {
            _collider.enabled = false;
            //_agent.enabled = false;
            _aiPath.enabled = false;
            OnDead?.Invoke(this);
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
                    OnLostAllTargets?.Invoke(this);
            }
        }

        public UnitHealth GetClosestTarget(UnitHealth[] targets = null)
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

                    if (onlyFreeTargets == true && target.IsTargeted == true) continue;

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

        public void SetDestionation(Vector3 destination, bool heldedPosition = false)
        {
            _aiPath.destination = destination;

            if (heldedPosition == true)
            {
                HeldedPosition = destination;
                OnHeldedPositionSetted?.Invoke();
            }
        }

        public void SwitchAttackType(AttackType newAttackType, bool callback = true)
        {
            if (newAttackType == CurrentAttackType) return;

            ResetAttackPoint();
            CurrentAttackType = newAttackType;
            _agroTrigger.radius = CurrentAttack.AgroRadius;

            if (callback == true)
                OnAttackTypeSwitched?.Invoke(this, CurrentAttackType);
        }

        public void ChargeAttackPoint(float time)
        {
            if (_chargingAttackCor != null || AttackCharged == true) return;

            _chargingAttackCor = StartCoroutine(ChargingAttackPoint(time));
        }

        private IEnumerator ChargingAttackPoint(float time)
        {
            yield return new WaitForSeconds(time);

            AttackCharged = true;
            _chargingAttackCor = null;
        }

        public void ResetAttackPoint()
        {
            if (_chargingAttackCor != null)
            {
                StopCoroutine(_chargingAttackCor);
                _chargingAttackCor = null;
            }

            AttackCharged = false;
        }

        public class Factory : PrefabFactory<CharacterStateMachine> { }
    }
}