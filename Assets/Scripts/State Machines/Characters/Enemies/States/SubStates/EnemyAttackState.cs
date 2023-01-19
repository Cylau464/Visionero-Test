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
    }
}