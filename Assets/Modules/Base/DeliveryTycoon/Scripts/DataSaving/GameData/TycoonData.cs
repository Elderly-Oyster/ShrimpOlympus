using System;
using System.Collections.Generic;

namespace Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameData
{
    [Serializable]
    public class TycoonData
    {
        public List<ContainerHoldersData> containersData = new() 
            {
                new ContainerHoldersData(),
                new ContainerHoldersData(),
                new ContainerHoldersData(),
                new ContainerHoldersData()
            };
        
        public int numberOfUnlockedUpgrades;
        public int maxNumberOfOrders = 1;
        public int experience;
        public int money = 1500;
        public int capacity = 1;
        public int level;
    }
}