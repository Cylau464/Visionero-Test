namespace States.Characters
{
    public class CharacterBattleState : CharacterState
    {
        public CharacterBattleState(CharacterStateMachine machine, CharacterStateFactory factory) : base(machine, factory)
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

            Machine.AnimationController.SetBattle(true);
            Machine.OnLostAllTargets += OnLostAllTargets;
        }

        public override void Exit()
        {
            Machine.Health.OnDead -= Dead;

            Machine.AnimationController.SetBattle(false);
            Machine.OnLostAllTargets -= OnLostAllTargets;
        }

        public override void InitializeSubState()
        {
            SetSubState(Factory.Chase());
        }

        public override void Update()
        {
            CheckSwitchStates();
        }

        private void OnLostAllTargets(CharacterStateMachine machine)
        {
            SwitchState(Factory.Neutral());
        }
    }
}
