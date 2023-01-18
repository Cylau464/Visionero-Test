using Units.Attributes;
using UnityEngine;

namespace States.Characters
{
    public class CharacterFindState : CharacterState
    {
        private float _checkClosestTargetInterval = 1f;
        private float _curCheckDelay;

        public CharacterFindState(CharacterStateMachine machine, CharacterStateFactory factory) : base(machine, factory)
        {
        }

        public override void CheckSwitchStates()
        {
            if (Machine.Target != null)
            {
                float distanceToTarget = GetDistanceToTarget();

                if (distanceToTarget > Machine.CurrentAttack.Distance)
                {
                    SwitchState(Factory.Chase());
                }
                else
                {
                    TryToAttack();
                }
            }
        }

        public override void Enter()
        {
            UpdateTarget();
            Machine.OnAttackTypeSwitched += OnAttackTypeSwitched;
        }

        public override void Exit()
        {
            Machine.OnAttackTypeSwitched -= OnAttackTypeSwitched;
        }

        public override void InitializeSubState()
        {

        }

        public override void Update()
        {
            if (_curCheckDelay <= Time.time)
                UpdateTarget();

            CheckSwitchStates();
            RotateToTarget();
        }

        protected override UnitHealth GetTarget()
        {
            _curCheckDelay = Time.time + _checkClosestTargetInterval;
            return base.GetTarget();
        }

        private void OnAttackTypeSwitched()
        {
            UpdateTarget();
        }
    }
}