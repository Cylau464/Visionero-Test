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
                float distanceToTarget = Vector3.Distance(Machine.transform.position, Machine.Target.transform.position) - Machine.Target.ExtraRangeForAttack;

                if (distanceToTarget > Machine.CurrentAttack.Distance)
                {
                    SwitchState(Factory.Chase());
                }
                else
                {
                    if (Machine.CurrentAttackType == AttackType.Range)
                        SwitchState(Factory.Aim());
                    else
                        SwitchState(Factory.Attack());
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
        }

        protected override UnitHealth GetTarget()
        {
            _curCheckDelay = Time.time + _checkClosestTargetInterval;
            return base.GetTarget();
        }

        private void UpdateTarget()
        {
            Machine.Target = GetTarget();
        }

        private void OnAttackTypeSwitched(CharacterStateMachine machine, AttackType attackType)
        {
            UpdateTarget();
        }
    }
}