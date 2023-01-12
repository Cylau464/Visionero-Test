using Pathfinding;
using UnityEngine;

namespace Units.Navigation
{
    public class GroupPathVisualizer : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private Seeker _seeker;

        private Path _path;

        private void OnEnable()
        {
            _seeker.pathCallback += OnPathCompleted;
        }

        private void OnDisable()
        {
            _seeker.pathCallback -= OnPathCompleted;
        }

        private void Update()
        {
            if (_path != null)
                UpdatePath();
        }

        private void OnPathCompleted(Path newPath)
        {
            _path = newPath;
            UpdatePath();
        }

        private void UpdatePath()
        {
            _lineRenderer.SetPositions(_path.vectorPath.ToArray());
        }
    }
}
