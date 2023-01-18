using Units.Attributes;
using UnityEngine;

namespace States.Characters.Enemy
{
    public class EnemyAttackState : CharacterAttackState
    {
        protected new EnemyStateMachine Machine => base.Machine as EnemyStateMachine;
        protected new EnemyStateFactory Factory => base.Factory as EnemyStateFactory;

        public EnemyAttackState(EnemyStateMachine machine, EnemyStateFactory factory, AttackType attackType) : base(machine, factory, attackType)
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