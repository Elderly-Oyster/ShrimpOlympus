using CodeBase.Core.Gameplay.Parcels;
using UnityEngine;

namespace CodeBase.Core.Gameplay.Configs
{
    [CreateAssetMenu(fileName = "ParcelConfig", menuName = "Configs/ParcelConfig", order = 0)]
    public class ParcelConfig : ScriptableObject
    {
        [SerializeField] private ParcelType serviceBuildingType;
        [SerializeField] private int deliveryCost;

        public ParcelType ParcelType => serviceBuildingType;

        public int DeliveryCost => deliveryCost;
    }
}