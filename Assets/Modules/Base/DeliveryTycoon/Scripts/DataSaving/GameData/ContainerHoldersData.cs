using System;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.BaseClasses.Parcels;

namespace Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameData
{
    [Serializable]
    public class ContainerHoldersData
    {
        public bool HasInitializedContainer;
        public ParcelType ParcelType;
        
        public ContainerHoldersData()
        {
            HasInitializedContainer = false;
            ParcelType = ParcelType.None;
        }
    }
}