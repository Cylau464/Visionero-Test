using System.Collections;
using UnityEngine;

namespace Units
{
    public class UnitHealthIndicator : HealthIndicator
    {
        [SerializeField] private UnitHealth _unitHealth;

        private void OnEnable()
        {
            _unitHealth.OnTakeDamage += OnUnitTakeDamage;
        }

        private void OnDisable()
        {
            _unitHealth.OnTakeDamage -= OnUnitTakeDamage;
        }

        private void Start()
        {
            InitializeBar();
        }

        protected override int GetTotalHealth()
        {
            return _unitHealth.Health;
        }

        protected override int GetTotalMaxHealth()
        {
            return _unitHealth.MaxHealth;
        }
    }
}