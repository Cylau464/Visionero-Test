using UnityEngine;

namespace States.Characters
{
    public class CharacterChaseState : CharacterState
    {
        private float _checkClosestTargetInterval = 1f;
        private float _curCheckDelay;
        private UnitHealth _target;

        public CharacterChaseState(CharacterStateMachine machine, CharacterStateFactory factory) : base(machine, factory)
        {
        }

        public override void CheckSwitchStates()
        {
            if (_target == null) return;

            float distanceToTarget = Vector3.Distance(Machine.transform.position, _target.transform.position) - _target.ExtraRangeForAttack;

            if (distanceToTarget <= Machine.AttackRadius)
            {
                Machine.AIPath.destination = Machine.transform.position;
                //Machine.Agent.SetDestination(Machine.transform.position);
                SwitchState(Factory.Attack(_target));
            }
        }

        public override void Enter()
        {
            _target = GetTarget();
        }

        public override void Exit()
        {
            if (_target != null)
                _target.RemoveTargetedUnit();
        }

        public override void InitializeSubState()
        {

        }

        public override void Update()
        {
            if (_curCheckDelay <= Time.time)
            {
                _target = GetTarget();

                if (_target == null) return;

                Machine.AIPath.destination = _target.transform.position;
                //Machine.Agent.SetDestination(_target.transform.position);
            }

            Machine.AnimationController.SetMoveSpeed(Machine.AIPath/*Agent*/.velocity.magnitude);
            CheckSwitchStates();
        }

        protected virtual UnitHealth GetTarget()
        {
            UnitHealth target = Machine.GetClosestTarget();
            _curCheckDelay = Time.time + _checkClosestTargetInterval;

            if (target != null)
                target.AddTargetedUnit();

            return target;
        }
    }
}