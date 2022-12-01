namespace States.Characters.Player
{
    public abstract class PlayerState : CharacterState
    {
        protected new PlayerStateMachine Machine => base.Machine as PlayerStateMachine;
        protected new PlayerStateFactory Factory => base.Factory as PlayerStateFactory;

        public PlayerState(PlayerStateMachine machine, PlayerStateFactory factory) : base(machine, factory) { }
    }
}