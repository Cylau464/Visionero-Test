namespace States.Characters.Player
{
    public class PlayerBattleState : CharacterBattleState
    {
        public PlayerBattleState(PlayerStateMachine machine, PlayerStateFactory factory) : base(machine, factory)
        {
        }

        public override void CheckSwitchStates()
        {
            if (Machine.IgnoreTargetsWhenMove == true
                && Machine.Agent.remainingDistance > Machine.StopIgnoringDestinationDistance)
                SwitchState(Factory.Neutral());
        }
    }
}
