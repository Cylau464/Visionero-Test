using UnityEngine;

namespace Units.Attributes
{
    public enum AttackType { Melee, Range, Both }

    [System.Serializable]
    public class AttackAttributes
    {
        [field: SerializeField] public float PrepareTime { get; private set; }
        [field: SerializeField, Tooltip("Distance of attack")] public float Distance { get; private set; }
    }

    [System.Serializable]
    public class RangeAttackAttrubtes : AttackAttributes
    {
        [field: SerializeField, Tooltip("If the target is closer than this distance, the unit cannot attack it in ranged combat.")]
        public float MinDistance { get; private set; }
        [field: SerializeField] public float AimTime { get; private set; }
        [field: SerializeField, Range(0, 100), Tooltip("In percent")] public int Accuracy { get; private set; }
    }

    [System.Serializable]
    public class MeleeAttackAttrubutes : AttackAttributes
    {

    }
}