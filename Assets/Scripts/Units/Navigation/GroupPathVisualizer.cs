using Pathfinding;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Units.Navigation
{
    public class GroupPathVisualizer : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private Seeker _seeker;
        [SerializeField] private DecalProjector _decalProjector;

        private Path _path;

        private void OnEnable()
        {
            _seeker.pathCallback += OnPathCompleted;
        }

        private void OnDisable()
        {
            _seeker.pathCallback -= OnPathCompleted;
        }

        private void OnPathCompleted(Path newPath)
        {
            _path = newPath;
            UpdatePath();
        }

        private void UpdatePath()
        {
            _lineRenderer.positionCount = _path.vectorPath.Count;
            _lineRenderer.SetPositions(_path.vectorPath.ToArray());

            if (_path.vectorPath.Count > 1)
            {
                if (_decalProjector.enabled == false)
                    _decalProjector.enabled = true;

                _decalProjector.transform.position = _path.vectorPath[_path.vectorPath.Count - 1];
            }
            else
            {
                if (_decalProjector.enabled == true)
                    _decalProjector.enabled = false;
            }
        }
    }
}
