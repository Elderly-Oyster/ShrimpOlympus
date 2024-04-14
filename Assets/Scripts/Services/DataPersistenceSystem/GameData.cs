using System;
using System.Collections.Generic;
using System.Globalization;
using Services.Backend;

namespace Services.DataPersistenceSystem
{
    [Serializable]
    public class GameData
    {
        public UserEnergyDto userEnergyDto;
        //other data ...

        public GameData()   
        {
            //other data initialize ...
            userEnergyDto = new UserEnergyDto
            {
                currentEnergy = 15,
                energyPerAD = 3,
                maxRenewableEnergy = 15,
                gameCost = 1,
                restartGameCost = 1,
                refreshmentCooldownSec = 600,   
                refreshmentEnergy = 1,
                updateDate = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)
            };
        }
    }
}