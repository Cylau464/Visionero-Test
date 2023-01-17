using States.Characters;
using States.Characters.Enemy;
using UnityEngine;
using Zenject;

namespace Units
{
    public class EnemyUnitsGroup : UnitsGroup
    {
        //[SerializeField] private Boat _boatPrefab;

        //private Boat _boat;

        protected override void Awake()
        {
            base.Awake();

            //foreach (CharacterStateMachine unit in _units)
            //    unit.gameObject.SetActive(false);

            //_boat = Instantiate(_boatPrefab, transform.position, transform.rotation);
            //transform.parent = _boat.transform;
            //_boat.OnInitialized += OnBoatInitialized;
            //_boat.OnArrived += OnArrived;
        }

        //private void OnDestroy()
        //{
        //    _boat.OnInitialized -= OnBoatInitialized;
        //    _boat.OnArrived -= OnArrived;
        //}

        //private void OnArrived()
        //{
        //    transform.parent = null;

        //    foreach (CharacterStateMachine unit in _units)
        //        (unit as EnemyStateMachine).Arrived();
        //}

        //private void OnBoatInitialized()
        //{
        //    //_boat.OnInitialized -= OnBoatInitialized;

        //    foreach (CharacterStateMachine unit in _units)
        //    {
        //        unit.gameObject.SetActive(true);
        //        unit.transform.localPosition = Vector3.zero;
        //    }
        //}

        public class Factory : PrefabFactory<EnemyUnitsGroup> { } 
    }
}