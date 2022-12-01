using System.Collections.Generic;
using Zenject;

namespace States
{
    public abstract class BaseStateFactory
    {
        protected BaseStateMachine Machine { get; private set; }
        protected State.ZenFactory StateFactory { get; private set; }

        public BaseStateFactory(BaseStateMachine machine, State.ZenFactory stateFactory)
        {
            Machine = machine;
            StateFactory = stateFactory;
        }

        public class ZenFactory
        {
            private readonly DiContainer _container;

            public ZenFactory(DiContainer container)
            {
                _container = container;
            }

            public T Create<T>(BaseStateMachine machine) where T : BaseStateFactory
            {
                List<object> args = new List<object>() { machine };
                return _container.Instantiate<T>(args);
            }
        }
    }
}