using Pathfinding;
using States.Characters;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Units.Navigation
{
    public class GroupPathVisualizer : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private DecalProjector _decalProjector;
        [SerializeField] private UnitsGroup _unitsGroup;

        [SerializeField] private Seeker _seeker;
        private Path _path;

        private void OnEnable()
        {
            //_unitsGroup.OnUnitDead += GetNewUnit;
            //_unitsGroup.OnGroupDead += Disable;
            //_seeker.pathCallback += OnPathCompleted;

            if (_seeker != null)
                _seeker.pathCallback += OnPathCompleted;
        }

        private void OnDisable()
        {
            //_seeker.pathCallback -= OnPathCompleted;

            //_unitsGroup.OnUnitDead -= GetNewUnit;
            //_unitsGroup.OnGroupDead -= Disable;

            if (_seeker != null)
                _seeker.pathCallback -= OnPathCompleted;
        }

        private void Start()
        {
            //GetNewUnit();
        }

        private void GetNewUnit()
        {
            CharacterStateMachine unit = _unitsGroup.CenterUnit;

            if (unit != null)
            {
                _seeker = unit.Seeker;
                _seeker.pathCallback += OnPathCompleted;
            }
        }

        private void Disable(UnitsGroup unitsGroup)
        {
            gameObject.SetActive(false);
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
