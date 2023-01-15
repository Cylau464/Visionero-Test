namespace Units
{
    public class UnitsGroupHealthIndicator : HealthIndicator
    {
        private UnitHealth[] _unitsHealth;

        private void OnDestroy()
        {
            foreach (UnitHealth unitHealth in _unitsHealth)
                unitHealth.OnTakeDamage -= OnUnitTakeDamage;
        }

        public void Init(UnitHealth[] unitsHealth)
        {
            foreach (UnitHealth unitHealth in unitsHealth)
                unitHealth.OnTakeDamage += OnUnitTakeDamage;

            _unitsHealth = unitsHealth;
            InitializeBar();
        }

        protected override int GetTotalHealth()
        {
            int totalHealth = 0;

            foreach (UnitHealth unitHealth in _unitsHealth)
                totalHealth += unitHealth.Health;

            return totalHealth;
        }

        protected override int GetTotalMaxHealth()
        {
            int totalHealth = 0;

            foreach (UnitHealth unitHealth in _unitsHealth)
                totalHealth += 1;// unitHealth.MaxHealth;

            return totalHealth;
        }
    }
}