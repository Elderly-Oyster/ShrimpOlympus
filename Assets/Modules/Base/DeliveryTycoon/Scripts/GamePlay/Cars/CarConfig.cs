using System.Collections.Generic;
using UnityEngine;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Cars
{
    [CreateAssetMenu(fileName = "CarType", menuName = "Cars/CarsConfig", order = 0)]
    public class CarConfig : ScriptableObject
    {
        public List<VehicleData> Vehicles;
    }
    
    [System.Serializable]
    public class VehicleData
    {
        public CarType Type;
        public GameObject ModelPrefab;
    }
}