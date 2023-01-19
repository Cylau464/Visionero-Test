using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

namespace Units.Modificators
{
    [System.Serializable]
    public class AccuracyModificator
    {
        [field: SerializeField, MinMaxSlider(0f, 100f)] public Vector2 Value { get; private set; }
        [field: SerializeField, MinMaxSlider(0f, 100f)] public Vector2 Threshold { get; private set; }

        public float GetThresholdPercent(float value)
        {
            float clamped = Mathf.Clamp(value, Threshold.x, Threshold.y);
            return (clamped - Threshold.x) / (Threshold.y - Threshold.x);
        }
    }
}
