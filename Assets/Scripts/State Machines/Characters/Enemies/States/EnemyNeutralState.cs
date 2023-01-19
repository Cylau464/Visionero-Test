using Units;
using UnityEngine;

namespace States.Characters.Enemy
{
    public class EnemyNeutralState : CharacterNeutralState
    {
        protected new EnemyStateMachine Machine => base.Machine as EnemyStateMachine;
        protected new EnemyStateFactory Factory => base.Factory as EnemyStateFactory;

        public EnemyNeutralState(EnemyStateMachine machine, EnemyStateFactory factory) : base(machine, factory)
        {
        }

        public override void Enter()
        {
            base.Enter();
            Machine.SetDestination(Machine.HeldedPosition);
        }

        //public override void InitializeSubState()
        //{
        //    SetSubState(Factory.Arrival());
        //}

        //public override void CheckSwitchStates()
        //{
        //    //Machine.IsArrived == true
        //    if (Machine.Targets.Count > 0 || Machine.BuildngsTargets.Length > 0)
        //        SwitchState(Factory.Battle());
        //}
    }
}