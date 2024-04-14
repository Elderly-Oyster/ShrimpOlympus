using System;
using System.Threading.Tasks;
using Core.Systems.DataPersistenceSystem;
using Cysharp.Threading.Tasks;
using Services.DataPersistenceSystem;
using Services.User;
using UnityEngine;

namespace Services.EnergyBar
{
    public class EnergyBarService : IDataPersistence
    {
        //private readonly IUserService _userService;   //TODO На расширение проекта и работу с FireBase
        //Все сервисы управляющие параметрами пользователя подписываются на его евент OnUserUpdate, который возращает 
        //обновлённую дату пользователя - Dto
        public Action<int, int> EnergyChanged;
        private readonly ConstantUserEnergy _constantUserEnergy = new();
        private int _currentEnergy;

        public async Task Init() 
        {
            // var user = await _userService.GetUser();
            // _constantUserEnergy = user.UserEnergyDto;
            // _currentEnergy = _constantUserEnergy.CurrentEnergy;
            // _userService.OnUserUpdated += GetSavedEnergy;

            //var oldCurrentEnergy = _currentEnergy;
            // _currentEnergy = _constantUserEnergy.CurrentEnergy + CalculateEnergyPerTime();
            // if (_currentEnergy > _constantUserEnergy.MaxRenewableEnergy)
            //     _currentEnergy = _constantUserEnergy.MaxRenewableEnergy;

            await UniTask.WaitForSeconds(0.25f);
            _currentEnergy = _constantUserEnergy.currentEnergy;
            if (_currentEnergy != _constantUserEnergy.maxRenewableEnergy) 
                AddSafeUserEnergy(CalculateEnergyPerTime());
        }

        // public void OnGameStarted()
        // {
        //     _constantUserEnergy.UpdateDate = DateTimeOffset.UtcNow.ToString();
        // }
        //
        // public void OnGameEnded()
        // {
        //     AddSafeUserEnergy(CalculateEnergyPerTime());
        // }
        
        // private void OnUserUpdated(User.User user) 
        // {
        //     _constantUserEnergy = user.UserEnergy;
        //     var oldCurrentEnergy = _currentEnergy;
        //     _currentEnergy = _constantUserEnergy.CurrentEnergy;
        //     EnergyChanged?.Invoke(oldCurrentEnergy, _currentEnergy);
        // }
        
        private int CalculateEnergyPerTime()
        {
            var energyRecovered = GetTotalSecondsSinceLastUpdate() / _constantUserEnergy.refreshmentCooldownSec;
            return energyRecovered;
        }
        
        public int GetTotalSecondsSinceLastUpdate() 
        {
            var timeSpanSinceLastUpdate = DateTime.UtcNow - _constantUserEnergy.updateDate;
            var totalSecondsSinceLastUpdate = (int)timeSpanSinceLastUpdate.TotalSeconds;
            Debug.Log($"total seconds - {totalSecondsSinceLastUpdate}   Update date - {_constantUserEnergy.updateDate}");
            return totalSecondsSinceLastUpdate;
        }
        
        public int IsEnoughEnergyToDoSmth()
        {
            var cost = _constantUserEnergy.startGameCost;
            if (_currentEnergy < cost) return -1;
            // && !_constantUserEnergy.InfiniteEnergyForever 
            // && DateTime.UtcNow > _constantUserEnergy.InfinityEnergyExpirationDate)
            return cost;
        }

        public void BuySmth()
        {
            var oldCurrentEnergy = _currentEnergy;
            _currentEnergy -= _constantUserEnergy.startGameCost;
            EnergyChanged?.Invoke(oldCurrentEnergy, _currentEnergy);
        }

        public void AddSafeUserEnergy(int energy)
        {
            _currentEnergy += energy;
            if (_currentEnergy > _constantUserEnergy.maxRenewableEnergy)
                _currentEnergy = _constantUserEnergy.maxRenewableEnergy;
            var oldCurrentEnergy = _currentEnergy;
            EnergyChanged?.Invoke(oldCurrentEnergy, _currentEnergy);
        }
        
        public ConstantUserEnergy GetEnergyBarDefaultSettings() => _constantUserEnergy;
        
        public int GetUserEnergy() => _currentEnergy;
        
        public void LoadData(GameData gameData) => _constantUserEnergy.SetData(gameData.userEnergyDto);
        public void SaveData(ref GameData gameData)
        {
            gameData.userEnergyDto.currentEnergy = _currentEnergy;
            gameData.userEnergyDto.updateDate = DateTimeOffset.UtcNow.ToString();
        }
    }
}