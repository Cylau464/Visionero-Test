using System;
using UnityEngine;

public class UnitHealth : MonoBehaviour
{
    [SerializeField, Tooltip("For melee attacks")] private float _extraRangeForAttack;

    public float ExtraRangeForAttack => _extraRangeForAttack;
    public int Health { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsTargeted => _targetedUnits > 0;
    private int _targetedUnits;
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

    public void AddTargetedUnit()
    {
        _targetedUnits++;
    }

    public void RemoveTargetedUnit()
    {
        _targetedUnits--;
    }
}