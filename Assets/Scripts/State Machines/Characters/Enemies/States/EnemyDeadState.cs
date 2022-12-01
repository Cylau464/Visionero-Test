namespace States.Characters.Enemy
{
    public class EnemyDeadState : CharacterDeadState
    {
        protected new EnemyStateMachine Machine => base.Machine as EnemyStateMachine;
        protected new EnemyStateFactory Factory => base.Factory as EnemyStateFactory;

        public EnemyDeadState(EnemyStateMachine machine, EnemyStateFactory factory) : base(machine, factory)
        {
        }
    }
}