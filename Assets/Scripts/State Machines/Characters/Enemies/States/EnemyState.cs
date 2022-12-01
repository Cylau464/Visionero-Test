namespace States.Characters.Enemy
{
    public abstract class EnemyState : CharacterState
    {
        protected new EnemyStateMachine Machine => base.Machine as EnemyStateMachine;
        protected new EnemyStateFactory Factory => base.Factory as EnemyStateFactory;

        public EnemyState(EnemyStateMachine machine, EnemyStateFactory factory) : base(machine, factory) { }
    }
}