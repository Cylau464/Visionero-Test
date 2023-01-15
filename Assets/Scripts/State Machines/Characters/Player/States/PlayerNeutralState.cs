namespace States.Characters.Player
{
    public class PlayerNeutralState : CharacterNeutralState
    {
        protected new PlayerStateMachine Machine => base.Machine as PlayerStateMachine;
        protected new PlayerStateFactory Factory => base.Factory as PlayerStateFactory;


        public PlayerNeutralState(PlayerStateMachine machine, PlayerStateFactory factory) : base(machine, factory)
        {
        }

        //public override void CheckSwitchStates()
        //{
        //    if (Machine.Targets.Count > 0)
        //    {
        //        //if (Machine.IgnoreTargetsWhenMove == true
        //        //    && Machine.AIPath.remainingDistance /*Machine.Agent.remainingDistance */<= Machine.StopIgnoringDestinationDistance)
        //            SwitchState(Factory.Battle());
        //    }
        //}
    }
}