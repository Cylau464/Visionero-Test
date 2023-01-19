using System;
using System.Collections.Generic;
using Units;
using UnityEngine;
using Zenject;

public class GameProgressHandler : MonoBehaviour
{
    private List<CapturePoint> _capturePoints = new List<CapturePoint>();
    private List<ControlledUnitsGroup> _defenderUnits = new List<ControlledUnitsGroup>();
    private List<UnitsGroup> _enemyGroups = new List<UnitsGroup>();
    private List<EnemySpawner> _enemySpawners = new List<EnemySpawner>();
    
    private int _enemyGroupsCount;
    private bool _gameEnded;

    public Action<bool> OnGameEnd { get; set; }

    [Inject]
    private void Consturct(List<EnemySpawner> enemySpawners, List<CapturePoint> capturePoints, List<ControlledUnitsGroup> defenderUnits)
    {
        _capturePoints = capturePoints;
        _defenderUnits = defenderUnits;
        _enemySpawners = new List<EnemySpawner>(enemySpawners);

        foreach(EnemySpawner spawner in _enemySpawners)
            _enemyGroupsCount = spawner.TotalEnemy;
    }

    private void OnEnable()
    {
        foreach (CapturePoint point in _capturePoints)
            point.OnCaptured += OnPointCaptured;

        foreach (ControlledUnitsGroup unitsGroup in _defenderUnits)
            unitsGroup.OnGroupDead += OnDefenderGroupDead;

        foreach (EnemySpawner spawner in _enemySpawners)
            spawner.OnEnemySpawned += AddEnemyGroup;
    }

    private void OnDisable()
    {
        foreach (CapturePoint point in _capturePoints)
            point.OnCaptured -= OnPointCaptured;

        foreach(EnemyUnitsGroup group in _enemyGroups)
            group.OnGroupDead -= OnGroupDead;

        foreach (ControlledUnitsGroup unitsGroup in _defenderUnits)
            unitsGroup.OnGroupDead -= OnDefenderGroupDead;

        foreach (EnemySpawner spawner in _enemySpawners)
            spawner.OnEnemySpawned -= AddEnemyGroup;
    }

    private void Lose()
    {
        if (_gameEnded == true) return;

        OnGameEnd?.Invoke(false);
        _gameEnded = true;
    }

    private void Win()
    {
        if (_gameEnded == true) return;

        OnGameEnd?.Invoke(true);
        _gameEnded = true;
    }

    private void OnPointCaptured(CapturePoint point)
    {
        point.OnCaptured -= OnPointCaptured;
        _capturePoints.Remove(point);

        if (_capturePoints.Count <= 0)
            Lose();
    }

    private void OnDefenderGroupDead(UnitsGroup group)
    {
        _defenderUnits.Remove(group as ControlledUnitsGroup);

        if (_defenderUnits.Count <= 0)
            Lose();
    }

    public void AddEnemyGroup(UnitsGroup group)
    {
        group.OnGroupDead += OnGroupDead;
        _enemyGroups.Add(group);
    }

    private void OnGroupDead(UnitsGroup group)
    {
        group.OnGroupDead -= OnGroupDead;

        if (--_enemyGroupsCount <= 0)
            Win();
    }
}