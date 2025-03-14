using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Cars
{
    public class CarPool
    {
        [Inject] private readonly CarFactory _carFactory;
        private readonly Dictionary<CarType, Queue<NPCCar>> _carPools = new();

        public NPCCar SpawnCar(CarType vehicleType, Vector3 position)
        {
            if (_carPools.TryGetValue(vehicleType, out var carsQueue) && carsQueue.Count > 0)
            {
                var car = carsQueue.Dequeue();
                car.gameObject.SetActive(true);
                return car;
            }
            
            return _carFactory.Create(vehicleType, position);
        }

        public void DespawnCar(NPCCar car, CarType vehicleType)
        {
            if (!_carPools.ContainsKey(vehicleType))
            {
                _carPools[vehicleType] = new Queue<NPCCar>();
            }

            car.gameObject.SetActive(false);
            _carPools[vehicleType].Enqueue(car);
        }
    }
}