using System;
using Units;
using UnityEngine;

namespace States.Characters.Player
{
    public class PlayerStateMachine : CharacterStateMachine
    {
        [SerializeField] private SelectedIndicator _selectedIndicator;

        protected new PlayerStateFactory States { get; private set; }

        protected override void InitializeState()
        {
            States = _factory.Create<PlayerStateFactory>(this);
            CurrentState = States.Neutral();
            CurrentState.Enter();
        }

        public override void SetUnitsGroup(UnitsGroup unitsGroup)
        {
            base.SetUnitsGroup(unitsGroup);

            _selectedIndicator.Init(unitsGroup as ControlledUnitsGroup);
        }
    }
}