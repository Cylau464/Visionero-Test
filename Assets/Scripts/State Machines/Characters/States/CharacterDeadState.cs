namespace States.Characters
{
    public class CharacterDeadState : CharacterState
    {
        public CharacterDeadState(CharacterStateMachine machine, CharacterStateFactory factory) : base(machine, factory)
        {
            IsRootState = true;
        }

        public override void CheckSwitchStates()
        {

        }

        public override void Enter()
        {
            Machine.Dead();
            Machine.AnimationController.Dead();
        }

        public override void Exit()
        {

        }

        public override void InitializeSubState()
        {

        }

        public override void Update()
        {

        }
    }
}