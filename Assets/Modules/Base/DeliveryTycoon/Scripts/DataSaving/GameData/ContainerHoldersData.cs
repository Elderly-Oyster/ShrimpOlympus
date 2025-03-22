using System;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.BaseClasses.Parcels;

namespace Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameData
{
    [Serializable]
    public class ContainerHoldersData
    {
        public bool hasInitializedContainer = false;
        public ParcelType parcelType = ParcelType.None;
    }
}