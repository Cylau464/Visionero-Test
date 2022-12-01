namespace States.Characters
{
    public class CharacterMoveState : CharacterState
    {
        public CharacterMoveState(CharacterStateMachine machine, CharacterStateFactory factory) : base(machine, factory) { }

        public override void CheckSwitchStates()
        {
            if (Machine.Agent.ReachedDestinationOrGaveUp() == true)
                SwitchState(Factory.Idle());
        }

        public override void Enter()
        {

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
            Machine.AnimationController.SetMoveSpeed(Machine.Agent.velocity.magnitude);
        }
    }
}