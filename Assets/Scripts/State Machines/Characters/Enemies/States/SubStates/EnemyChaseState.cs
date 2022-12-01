using UnityEngine;

namespace States.Characters.Enemy
{
    public class EnemyChaseState : CharacterChaseState
    {
        protected new EnemyStateMachine Machine => base.Machine as EnemyStateMachine;
        protected new EnemyStateFactory Factory => base.Factory as EnemyStateFactory;

        public EnemyChaseState(EnemyStateMachine machine, EnemyStateFactory factory) : base(machine, factory)
        {
        }

        protected override UnitHealth GetTarget()
        {
            UnitHealth target = base.GetTarget();

            if (target == null)
                target = Machine.GetClosestTarget(Machine.BuildngsTargets);

            return target;
        }
    }
}