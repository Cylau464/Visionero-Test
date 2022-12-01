using UnityEngine;
using Zenject;

namespace States
{
    public abstract class BaseStateMachine : MonoBehaviour
    {
        public State CurrentState;
        protected BaseStateFactory States { get; private set; }

        [Inject] protected BaseStateFactory.ZenFactory _factory;

        protected virtual void Start()
        {
            InitializeState();
        }

        protected virtual void Update()
        {
            CurrentState.UpdateStates();
        }

        protected abstract void InitializeState();

        protected virtual void OnDestroy()
        {
            if (CurrentState != null)
                CurrentState.ExitStates();
        }
    }
}
