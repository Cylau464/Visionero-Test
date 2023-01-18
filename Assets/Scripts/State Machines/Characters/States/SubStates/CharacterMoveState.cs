using Units.Attributes;

namespace States.Characters
{
    public class CharacterMoveState : CharacterState
    {
        public CharacterMoveState(CharacterStateMachine machine, CharacterStateFactory factory) : base(machine, factory) { }

        public override void CheckSwitchStates()
        {
            if (Machine.AIPath.reachedDestination/*Agent.ReachedDestinationOrGaveUp()*/ == true)
                SwitchState(Factory.Idle());
        }

        public override void Enter()
        {
            if (Machine.Combat.AttackType != AttackType.Melee)
                Machine.ResetAttackPoint(AttackType.Range);

            Machine.ChargeAttackPoint(AttackType.Melee, Machine.Combat.Melee.PrepareTime);
        }

        public override void Exit()
        {

        }

        public override void InitializeSubState()
        {

        }

        public override void Update()
        {
            CheckSwitchStates();
        }
    }
}