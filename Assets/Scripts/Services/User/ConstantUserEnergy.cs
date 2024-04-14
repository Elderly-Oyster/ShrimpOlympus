using System;
using Services.Backend;

namespace Services.User
{
    //TODO Сделать Serializable
    public class ConstantUserEnergy
    {
        public int currentEnergy { get; private set; }
        public int maxRenewableEnergy { get; private set; }
        public int startGameCost { get; private set; }
        public int restartGameCost { get; private set; }
        public int refreshmentCooldownSec { get; private set; }
        public int refreshmentEnergy { get; private set; }
        public int energyPerAd { get; private set; }
        public DateTimeOffset updateDate { get; private set; }
        
        // public DateTime InfinityEnergyExpirationDate { get; set; }
        // public bool InfiniteEnergyForever { get; private set; }
        
        
        // public ConstantUserEnergy SetEnergy(UserEnergyDto dto)
        // {
        //     //DateTime infinityEnergyExpirationDate = DateTime.MaxValue;
        //     // if (!string.IsNullOrEmpty(dto.infinite_energy_expiration_date) 
        //     //     && DateTime.TryParse(dto.infinite_energy_expiration_date, out infinityEnergyExpirationDate))
        //     // {
        //     // }
        //     return new ConstantUserEnergy
        //     {
        //         CurrentEnergy = dto.current_energy,
        //         MaxRenewableEnergy = dto.max_renewable_energy,
        //         MergeCost = dto.merge_cost,
        //         RefreshmentCooldownSec = dto.refreshment_cooldown_sec,
        //         RefreshmentEnergy = dto.refreshment_energy,
        //         UpdateDate = DateTimeOffset.Parse(dto.update_date),
        //         RestartGameCost = dto.duplicate_merge_cost,
        //         NothingCost = dto.nothing_merge_cost,
        //         EnergyPerAd = dto.energy_per_ad,
        //         //InfinityEnergyExpirationDate = infinityEnergyExpirationDate,
        //         //InfiniteEnergyForever = dto.infinite_energy_forever
        //     };
        // }
        
        public void SetData(UserEnergyDto dto)
        {
            currentEnergy = dto.currentEnergy;
            maxRenewableEnergy = dto.maxRenewableEnergy;
            startGameCost = dto.gameCost;
            restartGameCost = dto.restartGameCost;
            refreshmentCooldownSec = dto.refreshmentCooldownSec;
            refreshmentEnergy = dto.refreshmentEnergy;
            updateDate = DateTimeOffset.Parse(dto.updateDate);
            energyPerAd = dto.energyPerAD;
            //InfinityEnergyExpirationDate = infinityEnergyExpirationDate;
            //InfiniteEnergyForever = dto.infinite_energy_forever;
        }
    }
}