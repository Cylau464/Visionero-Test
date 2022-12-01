using UnityEngine;

namespace States.Characters.Enemy
{
    public class EnemyIdleState : CharacterIdleState
    {
        protected new EnemyStateMachine Machine => base.Machine as EnemyStateMachine;
        protected new EnemyStateFactory Factory => base.Factory as EnemyStateFactory;

        public EnemyIdleState(EnemyStateMachine machine, EnemyStateFactory factory) : base(machine, factory) { }
    }
}