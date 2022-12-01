using UnityEngine;

namespace States.Characters.Enemy
{
    public class EnemyWaitArrival : EnemyState
    {
        public EnemyWaitArrival(EnemyStateMachine machine, EnemyStateFactory factory) : base(machine, factory) { }

        public override void CheckSwitchStates()
        {
            
        }

        public override void Enter()
        {
            Machine.OnArrived += Arrived;
            Machine.Agent.enabled = false;
        }

        public override void Exit()
        {
            Machine.OnArrived -= Arrived;
            Machine.Agent.enabled = true;
            Machine.Agent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        }

        public override void InitializeSubState()
        {
            
        }

        public override void Update()
        {

        }

        private void Arrived()
        {
            SwitchState(Factory.Idle());
        }
    }
}