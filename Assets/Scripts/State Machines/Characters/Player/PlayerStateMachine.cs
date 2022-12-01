namespace States.Characters.Player
{
    public class PlayerStateMachine : CharacterStateMachine
    {
        protected new PlayerStateFactory States { get; private set; }

        protected override void InitializeState()
        {
            States = _factory.Create<PlayerStateFactory>(this);
            CurrentState = States.Neutral();
            CurrentState.Enter();
        }
    }
}