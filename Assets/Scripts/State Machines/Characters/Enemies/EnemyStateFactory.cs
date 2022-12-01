using UnityEngine;

namespace States.Characters.Enemy
{
    public class EnemyStateFactory : CharacterStateFactory
    {
        protected new EnemyStateMachine Machine => base.Machine as EnemyStateMachine;

        public EnemyStateFactory(EnemyStateMachine machine, State.ZenFactory stateFactory) : base(machine, stateFactory) { }

        public override State Idle()
        {
            return StateFactory.Create<EnemyIdleState>(Machine, this);
        }

        public override State Move()
        {
            return StateFactory.Create<EnemyMoveState>(Machine, this);
        }

        public override State Neutral()
        {
            return StateFactory.Create<EnemyNeutralState>(Machine, this);
        }

        public override State Dead()
        {
            return StateFactory.Create<EnemyDeadState>(Machine, this);
        }

        public override State Battle()
        {
            return StateFactory.Create<EnemyBattleState>(Machine, this);
        }

        public override State Chase()
        {
            return StateFactory.Create<EnemyChaseState>(Machine, this);
        }

        public override State Attack(UnitHealth target)
        {
            return StateFactory.Create<EnemyAttackState>(Machine, this, target);
        }

        public State Arrival()
        {
            return StateFactory.Create<EnemyWaitArrival>(Machine, this);
        }
    }
}