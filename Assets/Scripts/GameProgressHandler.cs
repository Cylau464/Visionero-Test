using System;
using System.Collections.Generic;
using Units;
using UnityEngine;
using Zenject;

public class GameProgressHandler : MonoBehaviour
{
    private List<UnitHealth> _defendedBuildings = new List<UnitHealth>();
    private List<ControlledUnitsGroup> _defenderUnits = new List<ControlledUnitsGroup>();
    private List<UnitsGroup> _enemyGroups = new List<UnitsGroup>();
    private int _enemyGroupsCount;
    private bool _gameEnded;
    private EnemySpawner _enemySpawner;

    public Action<bool> OnGameEnd { get; set; }

    [Inject]
    private void Consturct(EnemySpawner enemySpawner, List<UnitHealth> defendedBuildings, List<ControlledUnitsGroup> defenderUnits)
    {
        _enemySpawner = enemySpawner;
        _enemyGroupsCount = _enemySpawner.TotalEnemy;
        _defendedBuildings = defendedBuildings;
        _defenderUnits = defenderUnits;
    }

    private void OnEnable()
    {
        foreach (UnitHealth unit in _defendedBuildings)
            unit.OnDead += OnDefendedBuildingDestroyed;

        foreach (ControlledUnitsGroup unitsGroup in _defenderUnits)
            unitsGroup.OnGroupDead += OnDefenderGroupDead;

        _enemySpawner.OnEnemySpawned += AddEnemyGroup;
    }

    private void OnDisable()
    {
        foreach (UnitHealth unit in _defendedBuildings)
            unit.OnDead -= OnDefendedBuildingDestroyed;

        foreach(EnemyUnitsGroup group in _enemyGroups)
            group.OnGroupDead -= OnGroupDead;

        foreach (ControlledUnitsGroup unitsGroup in _defenderUnits)
            unitsGroup.OnGroupDead -= OnDefenderGroupDead;

        _enemySpawner.OnEnemySpawned -= AddEnemyGroup;
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

    private void OnDefendedBuildingDestroyed(UnitHealth unit)
    {
        unit.OnDead -= OnDefendedBuildingDestroyed;
        _defendedBuildings.Remove(unit);

        if (_defendedBuildings.Count <= 0)
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