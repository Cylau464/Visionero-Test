namespace States.Characters
{
    public class CharacterIdleState : CharacterState
    {
        public CharacterIdleState(CharacterStateMachine machine, CharacterStateFactory factory) : base(machine, factory) { }

        public override void CheckSwitchStates()
        {
            if (Machine.AIPath.reachedDestination/*Agent.ReachedDestinationOrGaveUp()*/ == false)
                SwitchState(Factory.Move());
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
            Machine.AnimationController.SetMoveSpeed(Machine.AIPath/*Agent*/.velocity.magnitude);
        }
    }
}