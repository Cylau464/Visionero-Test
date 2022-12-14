using States.Characters;
using System.Collections.Generic;
using Units;
using UnityEngine;
using Zenject;

namespace DI
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private UnitHealth[] _defendedBuildings;
        [SerializeField] private ControlledUnitsGroup[] _controlledUnitsGroups;
        [SerializeField] private GameProgressHandler _gameProgressHandler;

        public override void InstallBindings()
        {
            Container.BindInstance(_enemySpawner).AsSingle();
            Container.BindInstance(_gameProgressHandler).AsSingle();
            Container.BindInstances(_defendedBuildings);
            Container.BindInstances(_controlledUnitsGroups);

            Container.Bind<CharacterStateMachine.Factory>().AsSingle();
            Container.Bind<EnemyUnitsGroup.Factory>().AsSingle();
        }
    }
}