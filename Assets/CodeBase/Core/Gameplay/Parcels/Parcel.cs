using UnityEngine;

namespace CodeBase.Core.Gameplay.Parcels
{
    public class Parcel : MonoBehaviour
    {
        [SerializeField] private ParcelType _parcelType;
        
        [SerializeField] private int _experience;

        [SerializeField] private int deliveryCost = 30;

        public ParcelType ParcelType => _parcelType;

        public int Experience => _experience;

        public int DeliveryCost => deliveryCost;
    }
}