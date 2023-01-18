using NaughtyAttributes;
using UnityEngine;

namespace Units.Attributes
{
    [System.Serializable]
    public class UnitMovementAttributes
    {
        [field: SerializeField] public float MoveSpeed { get; private set; }
        [field: SerializeField] public float MaxChaseMoveSpeed { get; private set; }
        [field: SerializeField] public float RotationSpeed { get; private set; }
    }

    [System.Serializable]
    public class UnitCombatAttributes
    {
        [field: SerializeField] public AttackType AttackType { get; private set; }
        [field: SerializeField, Tooltip("Max distance from the position to chase a target")] public float MaxDistanceFromPosition { get; private set; }

        [field: SerializeField, AllowNesting, ShowIf(EConditionOperator.Or, nameof(AttackType), AttackType.Melee, AttackType.Both)]
        public MeleeAttackAttrubutes Melee { get; private set; }
        [field: SerializeField, AllowNesting, ShowIf(EConditionOperator.Or, nameof(AttackType), AttackType.Range, AttackType.Both)]
        public RangeAttackAttrubtes Range { get; private set; }
    }
}