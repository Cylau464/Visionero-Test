using UnityEngine;

namespace States.Characters.Enemy
{
    public class EnemyBattleState : CharacterBattleState
    {
        protected new EnemyStateMachine Machine => base.Machine as EnemyStateMachine;
        protected new EnemyStateFactory Factory => base.Factory as EnemyStateFactory;

        public EnemyBattleState(EnemyStateMachine machine, EnemyStateFactory factory) : base(machine, factory)
        {
        }
    }
}