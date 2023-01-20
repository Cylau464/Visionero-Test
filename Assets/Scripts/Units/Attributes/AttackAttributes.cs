using NaughtyAttributes;
using Units.Modificators;
using UnityEngine;

namespace Units.Attributes
{
    public enum AttackType { None, Melee, Range, Both }
    public enum DamageType { Target, AOE }

    [System.Serializable]
    public class AttackAttributes
    {
        [field: SerializeField] public float PrepareTime { get; private set; }
        [field: SerializeField, Tooltip("Distance of attack")] public float Distance { get; private set; }
        [field: SerializeField] public float AgroRadius { get; private set; }
        [field: SerializeField, Range(0, 100), Tooltip("In percent")] public int Accuracy { get; private set; }
    }

    [System.Serializable]
    public class RangeAttackAttrubtes : AttackAttributes
    {
        [field: SerializeField] public DamageType DamageType { get; private set; }
        [field: SerializeField] public Projectile Projectile { get; private set; }
        [field: SerializeField, Tooltip("If the target is closer than this distance, the unit cannot attack it in ranged combat.")]
        public float MinDistance { get; private set; }
        [field: SerializeField] public float AimTime { get; private set; }
        [field: SerializeField] public AccuracyModificator DistanceAccuracyModificator { get; private set; }
        [field: SerializeField] public AccuracyModificator MoveSpeedAccuracyModificator { get; private set; }
    }

    [System.Serializable]
    public class MeleeAttackAttrubutes : AttackAttributes
    {

    }
}