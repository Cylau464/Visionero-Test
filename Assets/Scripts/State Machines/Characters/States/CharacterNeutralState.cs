namespace States.Characters
{
    public class CharacterNeutralState : CharacterState
    {
        public CharacterNeutralState(CharacterStateMachine machine, CharacterStateFactory factory) : base(machine, factory)
        {
            IsRootState = true;
            InitializeSubState();
        }

        public override void CheckSwitchStates()
        {

        }

        public override void Enter()
        {
            Machine.Health.OnDead += Dead;
            Machine.OnFindTarget += OnFindTarget;

        }

        public override void Exit()
        {
            Machine.Health.OnDead -= Dead;
            Machine.OnFindTarget -= OnFindTarget;
        }

        public override void InitializeSubState()
        {
            if (Machine.Agent.ReachedDestinationOrGaveUp() == false)
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
            if (Machine.IgnoreTargetsWhenMove == true
                && Machine.Agent.remainingDistance > Machine.StopIgnoringDestinationDistance)
                return;

            SwitchState(Factory.Battle());
        }
    }
}