namespace States.Characters.Player
{
    public class PlayerIdleState : CharacterIdleState
    {
        protected new PlayerStateMachine Machine => base.Machine as PlayerStateMachine;
        protected new PlayerStateFactory Factory => base.Factory as PlayerStateFactory;

        public PlayerIdleState(PlayerStateMachine machine, PlayerStateFactory factory) : base(machine, factory)
        {
        }
    }
}