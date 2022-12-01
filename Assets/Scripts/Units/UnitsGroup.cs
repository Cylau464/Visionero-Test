using States.Characters;
using System;
using System.Collections;
using System.Collections.Generic;
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

        [Inject] private CharacterStateMachine.Factory _unitFactory;

        public Action<UnitsGroup> OnGroupDead { get; set; }

        protected virtual void Start()
        {
            Vector3[] positions = GetUnitsPositions(transform.position, _unitsCount);
            CharacterStateMachine unit;
            UnitHealth[] unitsHealth = new UnitHealth[_unitsCount];

            for (int i = 0; i < _unitsCount; i++)
            {
                unit = _unitFactory.Create(_unitPrefab);
                unit.transform.position = positions[i];
                unit.transform.parent = _unitsPivot;

                unit.OnDead += OnUnitDead;
                unit.OnFindTarget += OnUnitFindTarget;
                unit.OnLostAllTargets += OnUnitLostAllTargets;

                _units.Add(unit);
                unitsHealth[i] = unit.Health;
            }

            _healthIndicator.Init(unitsHealth);
        }

        private void OnDestroy()
        {
            foreach (CharacterStateMachine unit in _units)
            {
                unit.OnDead -= OnUnitDead;
                unit.OnFindTarget -= OnUnitFindTarget;
                unit.OnLostAllTargets -= OnUnitLostAllTargets;
            }
        }

        protected virtual void Update()
        {
            if(_units.Count > 0)
                _groupFollowObject.position = CalculateCenterOfUnits();
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

        protected void SetDestinations(Vector3[] destinations)
        {
            for (int i = 0; i < destinations.Length; i++)
            {
                if (i >= _units.Count) break;

                _units[i].Agent.SetDestination(destinations[i]);
            }
        }

        protected Vector3[] GetUnitsPositions(Vector3 centerPosition, int unitsCount)
        {
            Vector3[] positions = new Vector3[unitsCount];
            float sumRadius = _unitPrefab.Agent.radius * 2f;
            float minSeparationSq = _minDistanceBtwUnits * _minDistanceBtwUnits;

            for (int i = 0; i < positions.Length; i++)
            {
                Vector2 random = Random.insideUnitCircle;
                positions[i] = new Vector3(centerPosition.x + random.x, centerPosition.y, centerPosition.z + random.y);
            }

            for (int i = 0; i < positions.Length - 1; i++)
            {
                for (int j = i + 1; j < positions.Length; j++)
                {
                    if (i == j) continue;

                    Vector3 AB = positions[j] - positions[i];
                    float distance = AB.sqrMagnitude - minSeparationSq;
                    float minSepSq = Mathf.Min(distance, minSeparationSq);
                    distance -= minSepSq;

                    if (distance < (sumRadius * sumRadius) - 0.01f)
                    {
                        AB.Normalize();
                        AB *= (float)((sumRadius - Mathf.Sqrt(distance)) * 0.5f);
                        positions[j] += AB;
                        positions[i] -= AB;
                    }
                }
            }

            return positions;
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