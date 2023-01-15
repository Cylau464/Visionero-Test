using UnityEngine;
using Units.Attributes;

namespace Units
{
    [CreateAssetMenu(fileName = "Unit Config", menuName = "Units/Config")]
    public class UnitConfig : ScriptableObject
    {
        [field: SerializeField] public int Health { get; private set; }
        [field: SerializeField] public UnitMovementAttributes Movement { get; private set; }
        [field: SerializeField] public UnitCombatAttributes Combat { get; private set; }
    }
}