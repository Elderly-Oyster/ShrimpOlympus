using System;
using System.Collections.Generic;

namespace Modules.Base.DeliveryTycoon.Scripts.DataSaving.Game
{
    [Serializable]
    public class GameData
    {
        public List<ContainerHoldersData> containersData;
        public int level;
        public int experience;
        public int money;
        public int capacity;
        public int maxNumberOfOrders;
        public int numberOfUnlockedUpgrades;

        public GameData()
        {
            containersData = new List<ContainerHoldersData> {new(), new(), new(), new()};
            level = 0;
            experience = 0;
            money = 1500;
            capacity = 1;
            maxNumberOfOrders = 1;
            numberOfUnlockedUpgrades = 0;
        }
    }
}