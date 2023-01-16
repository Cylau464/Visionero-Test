using System;
using System.Diagnostics;
using Units.Attributes;

namespace States.Characters
{
    public class CharacterNeutralState : CharacterState
    {
        private bool _hasTarget;

        public CharacterNeutralState(CharacterStateMachine machine, CharacterStateFactory factory) : base(machine, factory)
        {
            IsRootState = true;
            InitializeSubState();
        }

        public override void CheckSwitchStates()
        {
            if(_hasTarget == true && Machine.AIPath.reachedDestination == true)
                SwitchState(Factory.Battle());
        }

        public override void Enter()
        {
            Machine.Health.OnDead += Dead;
            Machine.OnFindTarget += OnFindTarget;
            Machine.OnLostAllTargets += OnLostTarget;

            if (Machine.Combat.AttackType == AttackType.Melee)
                Machine.ChargeAttackPoint(Machine.Combat.Melee.PrepareTime);

            Machine.SetDestionation(Machine.HeldedPosition);
        }

        public override void Exit()
        {
            Machine.Health.OnDead -= Dead;
            Machine.OnFindTarget -= OnFindTarget;
            Machine.OnLostAllTargets -= OnLostTarget;
        }

        public override void InitializeSubState()
        {
            if (Machine.AIPath.reachedDestination == false)// Machine.Agent.ReachedDestinationOrGaveUp() == false)
                SetSubState(Factory.Move());
            else
                SetSubState(Factory.Idle());
        }

        public override void Update()
        {
            CheckSwitchStates();
        }

        private void OnFindTarget(CharacterStateMachine machine)
        {
            _hasTarget = true;

            //if (Machine.IgnoreTargetsWhenMove == true
            //    && Machine.AIPath.remainingDistance /*Machine.Agent.remainingDistance */> Machine.StopIgnoringDestinationDistance)
            //    return;

            //SwitchState(Factory.Battle());
        }

        private void OnLostTarget(CharacterStateMachine machine)
        {
            _hasTarget = false;
        }
    }
}