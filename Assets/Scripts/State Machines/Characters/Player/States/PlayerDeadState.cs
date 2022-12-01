namespace States.Characters.Player
{
    public class PlayerDeadState : CharacterDeadState
    {
        public PlayerDeadState(PlayerStateMachine machine, PlayerStateFactory factory) : base(machine, factory)
        {
        }
    }
}