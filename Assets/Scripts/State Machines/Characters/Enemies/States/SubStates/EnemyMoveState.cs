namespace States.Characters.Enemy
{
    public class EnemyMoveState : CharacterMoveState
    {
        protected new EnemyStateMachine Machine => base.Machine as EnemyStateMachine;
        protected new EnemyStateFactory Factory => base.Factory as EnemyStateFactory;

        public EnemyMoveState(EnemyStateMachine machine, EnemyStateFactory factory) : base(machine, factory) { }
    }
}