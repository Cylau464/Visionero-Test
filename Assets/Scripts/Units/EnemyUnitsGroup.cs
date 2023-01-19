using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Units
{
    public class EnemyUnitsGroup : UnitsGroup
    {
        [Inject] private List<CapturePoint> _capturePoints;
        public CapturePoint SelectedCapturePoint { get; private set; }

        private void Start()
        {
            foreach (CapturePoint point in _capturePoints)
                point.OnCaptured += OnPointCaptured;

            SelectCapturePoint();
        }

        private void SelectCapturePoint()
        {
            float minDistance = float.MaxValue;
            SelectedCapturePoint = null;

            foreach (CapturePoint point in _capturePoints)
            {
                float distance = Vector3.Distance(point.transform.position, transform.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    SelectedCapturePoint = point;
                }
            }

            if (SelectedCapturePoint != null)
            {
                Vector2 offset = Random.insideUnitCircle * SelectedCapturePoint.CaptureRadius;
                Vector3 destination = SelectedCapturePoint.transform.position;
                destination.x += offset.x;
                destination.z += offset.y;
                SetGroupDestination(destination);
            }
        }

        private void OnPointCaptured(CapturePoint point)
        {
            point.OnCaptured -= OnPointCaptured;
            _capturePoints.Remove(point);

            SelectCapturePoint();
        }

        public class Factory : PrefabFactory<EnemyUnitsGroup> { } 
    }
}