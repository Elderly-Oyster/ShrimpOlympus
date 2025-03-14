using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Cars
{
    public class NPCCarManager : MonoBehaviour
    {
        [Inject] private CarPool _carPool;
        [SerializeField] private GameObject spawn1;
        [SerializeField] private GameObject despawn1;
        [SerializeField] private GameObject spawn2;
        [SerializeField] private GameObject despawn2;
        [SerializeField] private GameObject spawn3;
        [SerializeField] private GameObject despawn3;
        [SerializeField] private GameObject spawn4;
        [SerializeField] private GameObject despawn4;
        
        private bool _carSpawnEnabled;
        private List<NPCCar> _cars = new();

        public void Initialize()
        {
            ProjectDiContainer.Instance.Inject(this);
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
                // yield return new WaitForSeconds(Random.Range(4f, 6f));
                // SpawnCar(RandomCarType(), spawn3.transform.position, despawn3.transform);
                // yield return new WaitForSeconds(Random.Range(4f, 6f));
                // SpawnCar(RandomCarType(), spawn4.transform.position, despawn4.transform);
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
        
        public void DespawnCar(NPCCar car, CarType vehicleType)
        {
            car.OnFinishPointReached -= DespawnCar;
            car.ResetParameters();
            _carPool.DespawnCar(car, vehicleType);
            _cars.Remove(car);
        }
        
        public void Shutdown()
        {
            _carSpawnEnabled = false;
            StopAllCoroutines();
            
        }
    }
}