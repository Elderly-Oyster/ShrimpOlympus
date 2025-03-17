using System;
using CodeBase.Core.Gameplay.Parcels;

namespace Modules.Base.DeliveryTycoon.Scripts.DataSaving.Game
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