using Units.Attributes;
using UnityEngine;

namespace States.Characters
{
    public class CharacterAttackState : CharacterState
    {
        private float _attackDelay;
        private bool _canAttack = true;
        private UnitHealth _target;

        public CharacterAttackState(CharacterStateMachine machine, CharacterStateFactory factory, UnitHealth target) : base(machine, factory)
        {
            _target = target;
        }

        public override void CheckSwitchStates()
        {
            if (_target == null) return;

            float distanceToTarget = Vector3.Distance(Machine.transform.position, _target.transform.position) - _target.ExtraRangeForAttack;
            
            if (_canAttack == true && distanceToTarget > Machine.CurrentAttack.Distance)
                SwitchState(Factory.Chase());
        }

        public override void Enter()
        {
            _target.OnDead += OnTargetDead;

            Machine.AnimationController.OnGiveDamage += GiveDamage;
            Machine.AnimationController.OnAttackEnd += AttackEnd;
            Machine.AnimationController.SetMoveSpeed(0f);
            _attackDelay = Time.time + Machine.CurrentAttack.PrepareTime;

        }

        public override void Exit()
        {
            if (_target != null)
            {
                _target.RemoveTargetedUnit();
                _target.OnDead -= OnTargetDead;
            }

            Machine.AnimationController.OnGiveDamage -= GiveDamage;
            Machine.AnimationController.OnAttackEnd -= AttackEnd;
        }

        public override void InitializeSubState()
        {

        }

        public override void Update()
        {
            CheckSwitchStates();

            if (_attackDelay <= Time.time && _canAttack == true)
            {
                _target = GetTarget();

                if (_target == null) return;

                Attack();
            }

            if (_target != null)
            {
                Vector3 direction = (_target.transform.position - Machine.transform.position).normalized;
                direction.y = 0f;
                Machine.transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        private void Attack()
        {
            _attackDelay = Time.time + Machine.CurrentAttack.PrepareTime;
            Machine.AnimationController.Attack();
            _canAttack = false;
        }

        private void GiveDamage()
        {
            if (_target != null)
            {
                if (Machine.CurrentAttackType == AttackType.Range)
                {
                    if(Machine.RangeAttack.Accuracy >= Random.value * 100)
                        _target.TakeHit();
                }
                else
                {
                    _target.TakeHit();
                }
            }
        }

        private void AttackEnd()
        {
            _canAttack = true;
        }

        private void OnTargetDead(UnitHealth target)
        {
            _target.OnDead -= OnTargetDead;
            _target = GetTarget();

            if(_target != null)
                _target.OnDead += OnTargetDead;
        }

        protected virtual UnitHealth GetTarget()
        {
            UnitHealth target = Machine.GetClosestTarget();

            if (target != null && target != _target)
                target.AddTargetedUnit();

            return target;
        }
    }
}