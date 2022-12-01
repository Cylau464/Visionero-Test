namespace States.Characters
{
    public abstract class CharacterState : State
    {
        protected new CharacterStateMachine Machine => base.Machine as CharacterStateMachine;
        protected new CharacterStateFactory Factory => base.Factory as CharacterStateFactory;

        public CharacterState(CharacterStateMachine machine, CharacterStateFactory factory) : base(machine, factory) { }

        protected virtual void Dead(UnitHealth health)
        {
            SwitchState(Factory.Dead());
        }
    }
}