using System;
using UnityEngine;

namespace Units
{
    public class UnitCommander : MonoBehaviour
    {
        [SerializeField] private LayerMask _navMeshMask;
        [SerializeField] private LayerMask _selectableMask;

        private Camera _camera;
        private Ray _ray;

        private ISelectable _selected;

        public static Action<Vector3> OnSetDestination { get; set; }

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) == true)
            {
                _ray = _camera.ScreenPointToRay(Input.mousePosition);
                
                if(TrySelectUnits() == false)
                    GetPointOnNavMesh();
            }
        }

        private void GetPointOnNavMesh()
        {
            if (Physics.Raycast(_ray, out RaycastHit hit, 100f, _navMeshMask) == true)
                OnSetDestination?.Invoke(hit.point);
        }

        private bool TrySelectUnits()
        {
            if (Physics.Raycast(_ray, out RaycastHit hit, 100f, _selectableMask) == true)
            {
                ISelectable curSelected = _selected;
                _selected = hit.collider.GetComponentInParent<ISelectable>();

                if (_selected != null)
                {
                    if (_selected.IsSelected == true)
                    {
                        return false;
                    }
                    else
                    {
                        if (curSelected != null && _selected != curSelected)
                            curSelected.Unselect();

                        _selected.Select();
                        return true;
                    }
                }
            }

            return false;
        }
    }
}