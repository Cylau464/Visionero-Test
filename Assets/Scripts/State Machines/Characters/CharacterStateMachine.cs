using System;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Zenject;
using Animations;

namespace States.Characters
{
    public abstract class CharacterStateMachine : BaseStateMachine
    {
        [SerializeField] private int _targetsCount;

        [SerializeField] protected NavMeshAgent _agent;
        public NavMeshAgent Agent => _agent;
        [SerializeField] protected UnitHealth _health;
        public UnitHealth Health => _health;
        [SerializeField] protected CharacterAnimationController _animationController;
        public CharacterAnimationController AnimationController => _animationController;
        [SerializeField] private Collider _collider;

        [Header("Agro Properties")]
        [SerializeField] private SphereCollider _agroTrigger;
        [SerializeField] private float _agroRadius = 5f;
        [SerializeField] private LayerMask _targetMask;
        [SerializeField] private bool _ignoreTargetsWhenMove;
        public bool IgnoreTargetsWhenMove => _ignoreTargetsWhenMove;
        [SerializeField] private float _stopIgnoringDestinationDistance = 2f;
        public float StopIgnoringDestinationDistance => _stopIgnoringDestinationDistance;


        [Header("Attack Properties")]
        [SerializeField] private float _attackRadius = 1f;
        public float AttackRadius => _attackRadius;
        [SerializeField] private int _damage;
        public int Damage => _damage;
        [SerializeField] private float _attackInterval = 1f;
        public float AttackInterval => _attackInterval;

        private List<UnitHealth> _targets = new List<UnitHealth>();
        public IList<UnitHealth> Targets => _targets.AsReadOnly();

        protected new CharacterStateFactory States { get; private set; }

        public Action<CharacterStateMachine> OnFindTarget { get; set; }
        public Action<CharacterStateMachine> OnLostAllTargets { get; set; }
        public Action<CharacterStateMachine> OnDead { get; set; }

        protected override void Start()
        {
            base.Start();
            _agroTrigger.radius = _agroRadius;
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

        public virtual void Dead()
        {
            _collider.enabled = false;
            _agent.enabled = false;
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
            _targetsCount++;

            if (_targets.Count == 1)
                OnFindTarget?.Invoke(this);
        }

        private void RemoveTargetFromList(UnitHealth target)
        {
            if (_targets.Contains(target) == true)
            {
                target.OnDead -= RemoveTargetFromList;
                _targets.Remove(target);
                _targetsCount--;

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

            foreach (UnitHealth target in targets)
            {
                if (target.IsDead == true) continue;

                float distance = Vector3.Distance(transform.position, target.transform.position);
                
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestTarget = target;
                }
            }

            return closestTarget;
        }

        public class Factory : PrefabFactory<CharacterStateMachine> { }
    }
}