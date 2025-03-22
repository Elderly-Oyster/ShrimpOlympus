using System.Collections.Generic;
using UnityEngine;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Cars.NPCCars
{
    public class CarPool
    {
        private readonly CarFactory _carFactory;
        private readonly Dictionary<CarType, Queue<NPCCarController>> _carPools = new();

        public CarPool(CarFactory carFactory)
        {
            _carFactory = carFactory;
        }

        public NPCCarController SpawnCar(CarType vehicleType, Vector3 position)
        {
            if (_carPools.TryGetValue(vehicleType, out var carsQueue) && carsQueue.Count > 0)
            {
                var car = carsQueue.Dequeue();
                car.gameObject.SetActive(true);
                return car;
            }
            
            return _carFactory.Create(vehicleType, position);
        }

        public void DespawnCar(NPCCarController carController, CarType vehicleType)
        {
            if (!_carPools.ContainsKey(vehicleType))
            {
                _carPools[vehicleType] = new Queue<NPCCarController>();
            }

            carController.gameObject.SetActive(false);
            _carPools[vehicleType].Enqueue(carController);
        }
    }
}