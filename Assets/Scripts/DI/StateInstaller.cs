using States;
using States.Characters;
using Zenject;

namespace DI
{
    public class StateInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindFactories();
        }

        private void BindFactories()
        {
            Container.Bind<BaseStateFactory.ZenFactory>().AsSingle();
            Container.Bind<State.ZenFactory>().AsSingle();
        }
    }
}