using System.Diagnostics;
using Zenject;

namespace States
{
    public abstract class State
    {
        private bool _isRootState;
        private BaseStateMachine _machine;
        private BaseStateFactory _factory;
        private State _currentSuperState;
        private State _currentSubState;

        protected bool IsRootState { set { _isRootState = value; } }
        protected BaseStateMachine Machine => _machine;
        protected BaseStateFactory Factory => _factory;

        public State(BaseStateMachine machine, BaseStateFactory factory)
        {
            _machine = machine;
            _factory = factory;
        }

        public class ZenFactory
        {
            private readonly DiContainer _container;

            public ZenFactory(DiContainer container)
            {
                _container = container;
            }

            public T Create<T>(params object[] arguments) where T : State
            {
                return _container.Instantiate<T>(arguments);
            }
        }

        public abstract void Enter();
        public abstract void Update();
        public abstract void Exit();
        public abstract void CheckSwitchStates();
        public abstract void InitializeSubState();

        public void UpdateStates()
        {
            Update();

            if (_currentSubState != null)
                _currentSubState.UpdateStates();
        }

        public void EnterStates()
        {
            Enter();
            
            if (_currentSubState != null)
                _currentSubState.EnterStates();
        }

        public void ExitStates()
        {
            Exit();

            if (_currentSubState != null)
                _currentSubState.ExitStates();
        }

        protected void SwitchState(State newState)
        {
            ExitStates();

            if (_isRootState == true)
                _machine.CurrentState = newState;
            else if (_currentSuperState != null)
                _currentSuperState.SwitchSubState(newState);

            newState.Enter();
        }

        protected void SetSuperState(State newSuperState)
        {
            _currentSuperState = newSuperState;
        }

        protected void SetSubState(State newSubState)
        {
            if (_currentSubState != null)
                _currentSubState.Exit();

            SwitchSubState(newSubState);
            _currentSubState.Enter();
        }

        private void SwitchSubState(State newSubState)
        {
            _currentSubState = newSubState;
            _currentSubState.SetSuperState(this);
        }
    }
}