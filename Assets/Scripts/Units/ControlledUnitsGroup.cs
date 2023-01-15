using System;
using System.Linq;
using UnityEngine;

namespace Units
{
    public class ControlledUnitsGroup : UnitsGroup, ISelectable
    {
        [Space]
        [SerializeField] private Collider _groupSelectTrigger;
        //[SerializeField] private SelectedIndicator _selectedIndicator;

        public bool IsSelected { get; private set; }

        public Action OnSelected { get; set; }
        public Action OnUnselected { get; set; }

        protected override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.Escape) && IsSelected == true)
                Unselect();
        }

        private void OnDestroy()
        {
            UnitCommander.OnSetDestination -= OnSetDestination;
        }

        public void Select()
        {
            // Убрать выбор мёртвых юнитов
            UnitCommander.OnSetDestination += OnSetDestination;
            IsSelected = true;
            OnSelected?.Invoke();
            //_selectedIndicator.Enable();
        }

        public void Unselect()
        {
            UnitCommander.OnSetDestination -= OnSetDestination;
            IsSelected = false;
            OnUnselected?.Invoke();
            //_selectedIndicator.Disable();
        }

        private void OnSetDestination(Vector3 destination)
        {
            Vector3[] destinations = GetUnitsPositions(destination, _units.Count);
            SetDestinations(destinations.ToList());
            Unselect();
        }

        protected override void GroupDead()
        {
            Unselect();
            _groupSelectTrigger.enabled = false;
            base.GroupDead();
        }
    }
}