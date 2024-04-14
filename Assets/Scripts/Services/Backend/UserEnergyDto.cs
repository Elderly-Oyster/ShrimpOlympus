using System;

namespace Services.Backend
{
    [Serializable]
    public class UserEnergyDto
    {
        public int currentEnergy;
        public int energyPerAD;
        public int gameCost;
        public int restartGameCost;
        public int maxRenewableEnergy;
        public int refreshmentCooldownSec;
        public int refreshmentEnergy;
        public string updateDate;
        //public string infinite_energy_expiration_date;
        //public bool infinite_energy_forever;
    }
}