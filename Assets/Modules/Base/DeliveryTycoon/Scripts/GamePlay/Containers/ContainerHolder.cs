using Modules.Base.DeliveryTycoon.Scripts.GamePlay.BaseClasses.Parcels;
using UnityEngine;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Containers
{
    public class ContainerHolder : MonoBehaviour
    {
        private ParcelType _parcelType;
        
        private bool _hasInitializedContainer;

        public bool HasInitializedContainer => _hasInitializedContainer;

        public ParcelType Type => _parcelType;

        public void SetActiveState(ParcelType parcelType)
        {
            _hasInitializedContainer = true;
            _parcelType = parcelType;
            Debug.Log($" {name} activated with ParcelType {Type}");
        }
        
    }
}