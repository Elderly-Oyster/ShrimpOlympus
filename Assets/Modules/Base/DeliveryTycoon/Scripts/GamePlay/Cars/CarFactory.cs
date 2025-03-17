using System.Linq;
using CodeBase.Core.Patterns.ObjectCreation;
using UnityEngine;
using VContainer;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Cars
{
    public class CarFactory : IFactory<CarType, Vector3, NPCCar>
    {
        private CarConfig _vehicleConfig;
        public CarFactory(CarConfig vehicleConfig)
        {
            _vehicleConfig = vehicleConfig;
        }
        
        public NPCCar Create(CarType vehicleType, Vector3 spawnPosition)
        {
            var vehicleData = _vehicleConfig.Vehicles.FirstOrDefault(v => v.Type == vehicleType);
            if (vehicleData == null)
            {
                Debug.LogError($"Vehicle type {vehicleType} not found in VehicleConfig!");
                return null;
            }

            var position = new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z);
            var newCar = Object.Instantiate(vehicleData.ModelPrefab, position, Quaternion.identity);
            if (newCar == null)
            {
                Debug.LogError($"Failed to instantiate vehicle of type {vehicleType}");
                return null;
            }
            
            var carController = newCar.AddComponent<NPCCar>();
            return carController;
        }
    }
}