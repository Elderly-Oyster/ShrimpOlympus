using System;
using System.Collections.Generic;
using CodeBase.Core.Systems.Save;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.Game;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Base.DeliveryTycoon.Scripts.DataSaving
{
    public class GameDataSystem : ISerializableDataSystem
    {
        private SaveSystem _saveSystem;
        
        private const string GameDataKey = "GameData";
        private ReactiveProperty<GameData> _gameDataProperty = new();

        public ReactiveCommand<Unit> OnContainerNeedsInitialization = new();
        public ReactiveCommand<int> OnUpdateCapacity = new();
        public ReactiveProperty<GameData> GameDataProperty => _gameDataProperty;
        public ReactiveCommand<int> OnItemBought { get; private set; } = new();
        public ReactiveCommand<int> OnUpdateMaxOrdersNumber = new();

        [Inject]
        public void Construct(SaveSystem saveSystem)
        {
            _saveSystem = saveSystem;
            _saveSystem.AddSystem(this);
            _gameDataProperty.Value = new GameData();
        }

        public int GetLevelData() => _gameDataProperty.Value?.level ?? 0;
        public int GetMoneyData() => _gameDataProperty.Value.money;

        // public int GetNumberOfUnlockedFeaturesData() => _gameDataProperty.Value.numberOfUnlockedUpgrades;
        //
        // public int GetCapacityData()=> _gameDataProperty.Value.capacity;
        //
        // public int GetMaxNumberOfOrdersData() => _gameDataProperty.Value.maxNumberOfOrders;

        public void InquireContainerInitialization()
        {
            OnContainerNeedsInitialization.Execute(Unit.Default);
        }

        public void SetLevelData(int newLevel)
        {
            if (newLevel < 0) return;
            _gameDataProperty.Value.level = newLevel;
        }

        public void SetExperience(int newExperience)
        {
            if (newExperience < 0) return;
            _gameDataProperty.Value.experience = newExperience;
        }

        public void SetMoneyData(int newMoney)
        {
            if (newMoney < 0) return;
            _gameDataProperty.Value.money = newMoney;
            OnItemBought.Execute(newMoney);
        }

        public void SetNumberOfUnlockedFeaturesData(int newNumberOfUnlockedFeatures)
        {
            if (newNumberOfUnlockedFeatures < 0) return;
            _gameDataProperty.Value.numberOfUnlockedUpgrades = newNumberOfUnlockedFeatures;
        }

        public void SetCapacityData(int newCapacity)
        {
            if (newCapacity < 0) return;    
            _gameDataProperty.Value.capacity = newCapacity;
            OnUpdateCapacity.Execute(newCapacity);
        }

        public void SetMaxNumberOfOrdersData(int newMaxNumberOfOrdersData)
        {
            if (newMaxNumberOfOrdersData < 0) return;
            _gameDataProperty.Value.maxNumberOfOrders = newMaxNumberOfOrdersData;
            OnUpdateMaxOrdersNumber.Execute(newMaxNumberOfOrdersData);
            _saveSystem.SaveData().Forget();
        }

        public List<ContainerHoldersData> GetContainerHoldersData() =>
            _gameDataProperty.Value?.containersData ?? new List<ContainerHoldersData>();

        public void SetContainersData(List<ContainerHoldersData> containersData)
        {
            if (containersData == null) return;
            _gameDataProperty.Value.containersData = containersData;
            _saveSystem.SaveData().Forget();
        }
        
        public void LoadData(SerializableDataContainer dataContainer)
        {
            if (dataContainer.TryGet(GameDataKey, out GameData loadedData))
            {
                _gameDataProperty.Value = loadedData;
                if (_gameDataProperty.Value == null)
                    _gameDataProperty.Value = new GameData();
            }
        }
        
        public void SaveData(SerializableDataContainer dataContainer)
        {
            dataContainer.SetData(GameDataKey, _gameDataProperty.Value);
        }
    }
}