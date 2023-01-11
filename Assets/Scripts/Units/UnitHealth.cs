using System;
using UnityEngine;

public class UnitHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth;
    [SerializeField] private float _extraRangeForAttack;

    public float ExtraRangeForAttack => _extraRangeForAttack;
    public int MaxHealth => _maxHealth;
    public int Health { get; private set; }
    public bool IsDead { get; private set; }
    public bool IsTargeted => _targetedUnits > 0;
    private int _targetedUnits;

    public Action<int> OnTakeDamage { get; set; }
    public Action<UnitHealth> OnDead { get; set; }

    private void Start()
    {
        Health = _maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (Health <= 0 || damage <= 0) return;

        Health = Mathf.Max(0, Health - damage);
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