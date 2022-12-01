using UnityEngine;

namespace Units
{
    public class ControlledUnitsGroup : UnitsGroup, ISelectable
    {
        [Space]
        [SerializeField] private Collider _groupSelectTrigger;
        [SerializeField] private SelectedIndicator _selectedIndicator;

        public bool IsSelected { get; private set; }

        protected override void Update()
        {
            base.Update();

            if (Input.GetKeyDown(KeyCode.Escape) && IsSelected == true)
                Unselect();
        }

        public void Select()
        {
            UnitCommander.OnSetDestination += OnSetDestination;
            IsSelected = true;
            _selectedIndicator.Enable();
        }

        public void Unselect()
        {
            UnitCommander.OnSetDestination -= OnSetDestination;
            IsSelected = false;
            _selectedIndicator.Disable();
        }

        private void OnSetDestination(Vector3 destination)
        {
            Vector3[] destinations = GetUnitsPositions(destination, _units.Count);
            SetDestinations(destinations);
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