using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Animations
{
    public class CharacterAnimationEventsCaller : AnimationEventsCaller
    {
        [SerializeField] private UnityEvent _giveDamage;
        [SerializeField] private UnityEvent _attackEnd;

        private void GiveDamage()
        {
            _giveDamage?.Invoke();
        }

        private void AttackEnd()
        {
            _attackEnd?.Invoke();
        }
    }
}