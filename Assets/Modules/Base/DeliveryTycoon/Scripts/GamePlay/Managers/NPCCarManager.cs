using System;
using System.Collections;
using System.Collections.Generic;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Cars.NPCCars;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers
{
    public class NPCCarManager : MonoBehaviour
    {
        private CarPool _carPool;
        [SerializeField] private GameObject spawn1;
        [SerializeField] private GameObject despawn1;
        [SerializeField] private GameObject spawn2;
        [SerializeField] private GameObject despawn2;
        
        private bool _carSpawnEnabled;
        private List<NPCCarController> _cars = new();

        [Inject]
        public void Construct(CarPool carPool)
        {
            _carPool = carPool;
        }

        public void StartSpawnCars()
        {
            _carSpawnEnabled = true;
            StartCoroutine(SpawnDecorationCars());
        }
        
        private IEnumerator SpawnDecorationCars()
        {
            while (_carSpawnEnabled)
            {
                SpawnCar(RandomCarType(), spawn1.transform.position, despawn1.transform);
                yield return new WaitForSeconds(Random.Range(8f, 10f));
                SpawnCar(RandomCarType(), spawn2.transform.position, despawn2.transform);
            }
        }
        
        private void SpawnCar(CarType vehicleType, Vector3 spawnPosition, Transform targetPosition)
        {
            var car = _carPool.SpawnCar(vehicleType, spawnPosition);
            car.transform.SetParent(targetPosition.parent);
            car.Type = vehicleType;
            _cars.Add(car);
            car.OnFinishPointReached += DespawnCar;
            car.transform.position = spawnPosition;
            car.MoveTo(targetPosition.transform.position, targetPosition.rotation);
        }
        
        private CarType RandomCarType()
        {
            Array values = Enum.GetValues(typeof(CarType));
            return (CarType)values.GetValue(Random.Range(0, values.Length));
        }
        
        public void DespawnCar(NPCCarController carController, CarType vehicleType)
        {
            carController.OnFinishPointReached -= DespawnCar;
            carController.ResetParameters();
            _carPool.DespawnCar(carController, vehicleType);
            _cars.Remove(carController);
        }
        
        // public void Shutdown()
        // {
        //     _carSpawnEnabled = false;
        //     StopAllCoroutines();
        // }
    }
}