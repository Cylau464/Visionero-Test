using UnityEngine;
using UnityEngine.Events;

namespace Animations
{
    public class AnimationEventsCaller : MonoBehaviour
    {
        [SerializeField] private UnityEvent _action;

        private void Action()
        {
            _action?.Invoke();
        }
    }
}