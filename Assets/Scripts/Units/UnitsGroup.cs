using States.Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Sprites;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Units
{
    public class UnitsGroup : MonoBehaviour
    {
        [Header("Spawn Units Properties")]
        [SerializeField] private CharacterStateMachine _unitPrefab;
        [SerializeField] private Transform _unitsPivot;
        [SerializeField] private int _unitsCount = 3;
        [SerializeField] private float _minDistanceBtwUnits = .5f;
        [Space]
        [SerializeField] private UnitsGroupHealthIndicator _healthIndicator;
        [SerializeField] private Transform _groupFollowObject;


        protected List<CharacterStateMachine> _units = new List<CharacterStateMachine>();
        private bool _groupHaveTarget;
        private Vector3 _centerPosition;

        [Inject] private CharacterStateMachine.Factory _unitFactory;

        public Action<UnitsGroup> OnGroupDead { get; set; }

        private Vector3[] _positions = new Vector3[] { };
        private float _radius;

        protected virtual void Start()
        {
            SpawnUnits();
        }

        private void OnDestroy()
        {
            foreach (CharacterStateMachine unit in _units)
            {
                unit.OnDead -= OnUnitDead;
                //unit.OnFindTarget -= OnUnitFindTarget;
                //unit.OnLostAllTargets -= OnUnitLostAllTargets;
            }
        }

        protected virtual void Update()
        {
            if(_units.Count > 0)
                _groupFollowObject.position = CalculateCenterOfUnits();
        }

        private void SpawnUnits()
        {
            Vector3[] positions = GetUnitsPositions(transform.position, _unitsCount);
            CharacterStateMachine unit;
            UnitHealth[] unitsHealth = new UnitHealth[_unitsCount];

            for (int i = 0; i < _unitsCount; i++)
            {
                unit = _unitFactory.Create(_unitPrefab);
                unitsHealth[i] = InitUnit(unit, positions[i], i);
            }

            _healthIndicator.Init(unitsHealth);
        }

        private UnitHealth InitUnit(CharacterStateMachine unit, Vector3 position, int index)
        {
            unit.transform.position = position;
            unit.transform.parent = _unitsPivot;
            unit.name += " " + index;
            unit.SetUnitsGroup(this);

            unit.OnDead += OnUnitDead;
            //unit.OnFindTarget += OnUnitFindTarget;
            //unit.OnLostAllTargets += OnUnitLostAllTargets;

            _units.Add(unit);
            return unit.Health;
        }

        private void OnUnitDead(CharacterStateMachine unit)
        {
            unit.OnDead -= OnUnitDead;
            _units.Remove(unit);

            if (_units.Count <= 0)
                GroupDead();
        }

        protected virtual void GroupDead()
        {
            OnGroupDead?.Invoke(this);
        }

        private void OnUnitFindTarget(CharacterStateMachine unit)
        {
            if (_groupHaveTarget == true) return;

            UnitHealth[] targets = new UnitHealth[unit.Targets.Count];
            unit.Targets.CopyTo(targets, 0);

            foreach (CharacterStateMachine u in _units)
            {
                if (u == unit) continue;

                u.AddTargetsToList(targets);
            }

            _groupHaveTarget = true;
        }

        private void OnUnitLostAllTargets(CharacterStateMachine unit)
        {
            if (_groupHaveTarget == false) return;

            UnitHealth[] targets = new UnitHealth[] { };

            foreach (CharacterStateMachine u in _units)
            {
                if (u == unit) continue;

                if (u.Targets.Count > 0)
                {
                    targets = new UnitHealth[u.Targets.Count];
                    u.Targets.CopyTo(targets, 0);
                }
            }

            //if (targets.Length > 0)
            //{
            //    unit.AddTargetsToList(targets);
            //    return;
            //}

            _groupHaveTarget = false;
        }

        protected void SetDestinations(List<Vector3> destinations)
        {
            float distance, curDistance = float.MaxValue;
            int unitIndex = 0;
            float time = Time.realtimeSinceStartup;
            List<CharacterStateMachine> units = new List<CharacterStateMachine>(_units);
            units = units.OrderBy(x => (x.transform.position - destinations[0]).sqrMagnitude).ToList();
            destinations.RemoveAt(0);
            destinations = destinations.OrderByDescending(x => (x - units[units.Count - 1].transform.position).sqrMagnitude).ToList();

            for (int i = 0; i < destinations.Count; i++)
            {
                if (i >= _units.Count) break;

                for (int j = 0; j < units.Count; j++)
                {
                    distance = (destinations[i] - units[j].transform.position).sqrMagnitude;

                    if (distance < curDistance)
                    {
                        curDistance = distance;
                        unitIndex = j;
                    }
                }

                units[unitIndex].AIPath.destination = destinations[i];
                units.RemoveAt(unitIndex);
                curDistance = float.MaxValue;
                unitIndex = 0;
                //_units[i].AIPath.destination = destinations[i];//Agent.SetDestination(destinations[i]);
            }

            Debug.LogWarning("Time spent accumulating units: " + (Time.realtimeSinceStartup - time));
        }

        /// <summary>
        /// </summary>
        /// <param name="centerPosition"></param>
        /// <param name="unitsCount"></param>
        /// <returns>Array with first element equals centerPosition</returns>
        protected Vector3[] GetUnitsPositions(Vector3 centerPosition, int unitsCount)
        {
            _centerPosition = centerPosition;
            List<Vector3> positions = new List<Vector3>(unitsCount);
            int iterations = 0;
            float time = Time.realtimeSinceStartup;
            float sumRadius = _unitPrefab.AIPath.radius/* Agent.radius */* 2f;
            float minSeparationSq = _minDistanceBtwUnits * _minDistanceBtwUnits;
            positions.Add(centerPosition);

            for (int i = 0; i < unitsCount; i++)
            {
                Vector2 random = Random.insideUnitCircle;
                positions.Add(new Vector3(centerPosition.x + random.x, centerPosition.y, centerPosition.z + random.y));
            }

            while (true)
            {
                bool changed = false;
                positions.Sort(SortByDistanceToCenter);

                for (int i = 0; i < positions.Count - 1; i++)
                {
                    for (int j = i + 1; j < positions.Count; j++)
                    {
                        if (i == j) continue;

                        Vector3 AB = positions[j] - positions[i];
                        float distance = AB.sqrMagnitude - minSeparationSq;
                        float minSepSq = Mathf.Min(distance, minSeparationSq);
                        distance -= minSepSq;

                        if (distance < (sumRadius * sumRadius) - 0.01f)
                        {
                            changed = true;
                            AB.Normalize();
                            AB *= (float)((sumRadius - Mathf.Sqrt(distance)) * 0.5f);
                            positions[j] += AB;
                            positions[i] -= AB;
                        }
                    }
                }

                iterations++;

                if (changed == false) break;
            }

            Debug.Log(iterations + " iterations by time: " + (Time.realtimeSinceStartup - time));

            _positions = positions.ToArray();
            _radius = _unitPrefab.AIPath/*Agent*/.radius;
            return positions.ToArray();
        }

        private int SortByDistanceToCenter(Vector3 v1, Vector3 v2)
        {
            float distance1 = Vector3.Distance(v1, _centerPosition);
            float distance2 = Vector3.Distance(v2, _centerPosition);
            
            if (distance1 < distance2)
                return -1;
            else if (distance1 > distance2)
                return 1;
            else
                return 0;
        }

        private void OnDrawGizmos()
        {
            foreach (Vector3 pos in _positions)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(pos, _radius);
            }

            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(_centerPosition, Vector3.one);
        }

        private Vector3 CalculateCenterOfUnits()
        {
            Vector3 center = Vector3.zero;

            foreach (CharacterStateMachine unit in _units)
                center += unit.transform.position;

            center /= _units.Count;

            return center;
        }
    }
}