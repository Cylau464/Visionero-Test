using System;
using Zenject;

namespace States.Characters.Enemy
{
    public class EnemyStateMachine : CharacterStateMachine
    {
        [Inject] private UnitHealth[] _buildingsTargets;
        public UnitHealth[] BuildngsTargets => _buildingsTargets;
        public bool IsArrived { get; private set; }

        protected new EnemyStateFactory States;

        public Action OnArrived { get; set; }

        protected override void InitializeState()
        {
            States = _factory.Create<EnemyStateFactory>(this);
            CurrentState = States.Neutral();
            CurrentState.Enter();
        }

        public void Arrived()
        {
            IsArrived = true;
            OnArrived?.Invoke();
        }
    }
}