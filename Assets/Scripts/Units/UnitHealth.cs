using States.Characters;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitHealth : MonoBehaviour
{
    [SerializeField, Tooltip("For melee attacks")] private float _extraRangeForAttack;

    public float ExtraRangeForAttack => _extraRangeForAttack;
    public int Health { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsTargeted => _targetedUnits.Count > 0;
    public int CountOfTargeted => _targetedUnits.Count;
    private List<CharacterStateMachine> _targetedUnits = new List<CharacterStateMachine>();
    private int _maxHealth;

    public Action<int> OnTakeDamage { get; set; }
    public Action<UnitHealth> OnDead { get; set; }

    public void Init(int maxHealth)
    {
        Health = _maxHealth = maxHealth;
    }

    public void TakeHit()
    {
        if (Health <= 0) return;

        Health = Mathf.Max(0, Health - 1);
        OnTakeDamage?.Invoke(Health);

        if (Health <= 0)
        {
            IsDead = true;
            OnDead?.Invoke(this);
        }
    }

    public void AddTargetedUnit(CharacterStateMachine unit)
    {
        _targetedUnits.Add(unit);
    }

    public void RemoveTargetedUnit(CharacterStateMachine unit)
    {
        _targetedUnits.Remove(unit);
    }

    public bool IsMyTarget(CharacterStateMachine unit)
    {
        return _targetedUnits.Contains(unit);
    }
}