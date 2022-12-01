namespace States.Characters.Player
{
    public class PlayerMoveState : CharacterMoveState
    {
        protected new PlayerStateMachine Machine => base.Machine as PlayerStateMachine;
        protected new PlayerStateFactory Factory => base.Factory as PlayerStateFactory;

        public PlayerMoveState(PlayerStateMachine machine, PlayerStateFactory factory) : base(machine, factory)
        {
        }
    }
}