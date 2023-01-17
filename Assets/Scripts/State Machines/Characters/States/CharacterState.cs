using UnityEngine;

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

        protected virtual UnitHealth GetTarget()
        {
            UnitHealth target = Machine.GetClosestTarget();

            if (target != null && target != Machine.Target)
            {
                if (Machine.Target != null)
                    Machine.Target.RemoveTargetedUnit();

                target.AddTargetedUnit();
            }

            return target;
        }
    }
}