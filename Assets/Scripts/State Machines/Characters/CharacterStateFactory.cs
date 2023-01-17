namespace States.Characters
{
    public class CharacterStateFactory : BaseStateFactory
    {
        protected new CharacterStateMachine Machine => base.Machine as CharacterStateMachine;

        public CharacterStateFactory(CharacterStateMachine machine, State.ZenFactory stateFactory) : base(machine, stateFactory) { }

        public virtual State Neutral()
        {
            return StateFactory.Create<CharacterNeutralState>(Machine, this);
        }

        public virtual State Dead()
        {
            return StateFactory.Create<CharacterDeadState>(Machine, this);
        }

        public virtual State Battle()
        {
            return StateFactory.Create<CharacterBattleState>(Machine, this);
        }

        public virtual State Move()
        {
            return StateFactory.Create<CharacterMoveState>(Machine, this);
        }

        public virtual State Idle()
        {
            return StateFactory.Create<CharacterIdleState>(Machine, this);
        }

        public virtual State Chase()
        {
            return StateFactory.Create<CharacterChaseState>(Machine, this);
        }

        public virtual State Attack()
        {
            return StateFactory.Create<CharacterAttackState>(Machine, this);
        }

        public virtual State Aim()
        {
            return StateFactory.Create<CharacterAimState>(Machine, this);
        }

        public virtual State FindTarget()
        {
            return StateFactory.Create<CharacterFindState>(Machine, this);
        }
    }
}