using System;
using System.Collections;
using System.Linq;
using Units;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Wave[] _waves;
    [SerializeField] private Transform _spawnCenterPoint;
    [SerializeField] private float _spawnRadius = 30f;

    private int _enemyGroupsLeft;

    [Inject] private EnemyUnitsGroup.Factory _enemyGroupFactory;

    public int TotalEnemy => _waves.Sum(x => x.EnemyForSpawn.Length);

    public Action<EnemyUnitsGroup> OnEnemySpawned { get; set; }

    private void Start()
    {
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        Vector3 spawnPoint;
        EnemyUnitsGroup enemy;

        for (int i = 0; i < _waves.Length; i++)
        {
            foreach (EnemyUnitsGroup enemyPrefab in _waves[i].EnemyForSpawn)
            {
                spawnPoint = _spawnCenterPoint.position + GetRandomPointOnCircle() * _spawnRadius;
                enemy = _enemyGroupFactory.Create(enemyPrefab);
                enemy.transform.position = spawnPoint;

                OnEnemySpawned?.Invoke(enemy);

                if (_waves[i].WaitWaveDeadBeforeNext == true)
                {
                    enemy.OnGroupDead += OnEnemyGroupDead;
                    _enemyGroupsLeft++;
                }
            }

            if (_waves[i].WaitWaveDeadBeforeNext == true)
            {
                while (_enemyGroupsLeft > 0)
                    yield return null;
            }
            else
            {
                yield return new WaitForSeconds(_waves[i].IntervalBeforeNext);
            }
        }
    }

    private void OnEnemyGroupDead(UnitsGroup group)
    {
        group.OnGroupDead -= OnEnemyGroupDead;
        _enemyGroupsLeft--;
    }

    private Vector3 GetRandomPointOnCircle()
    {
        Vector2 insideCircleNormalized;

        do
            insideCircleNormalized = Random.insideUnitCircle.normalized;
        while (insideCircleNormalized == Vector2.zero);

        return new Vector3(insideCircleNormalized.x, 0f, insideCircleNormalized.y);
    }
}

[Serializable]
public struct Wave
{
    public EnemyUnitsGroup[] EnemyForSpawn;
    public bool WaitWaveDeadBeforeNext;
    public float IntervalBeforeNext;
}