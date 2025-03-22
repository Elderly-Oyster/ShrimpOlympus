using System;
using System.Collections.Generic;
using CodeBase.Core.Systems.Save;
using Cysharp.Threading.Tasks;
using MediatR;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameData;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Containers;
using R3;
using UnityEngine;
using VContainer;
using static Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameDataSystem.GameDataSystemOperations;
using Unit = R3.Unit;

namespace Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameDataSystem
{
    public class GameDataSystem : ISerializableDataSystem
    {
        private SaveSystem _saveSystem;
        private Mediator _mediator;
        
        private const string GameDataKey = "GameData";
        private  ReactiveProperty<GameData.GameData> _gameDataProperty = new();

        public readonly ReactiveCommand<Unit> OnContainerNeedsInitialization = new();
        public readonly ReactiveCommand<int> OnUpdateCapacity = new();
        public ReactiveProperty<GameData.GameData> GameDataProperty => _gameDataProperty;
        public ReactiveCommand<int> OnItemBought { get; private set; } = new();
        public readonly ReactiveCommand<int> OnUpdateMaxOrdersNumber = new();

        [Inject]
        public void Construct(SaveSystem saveSystem, Mediator mediator)
        {
            _saveSystem = saveSystem;
            _mediator = mediator;
            _saveSystem.AddSystem(this);
            if (_gameDataProperty == null) 
                _gameDataProperty.Value = new GameData.GameData();
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

        public void SetNumberOfUnlockedUpgradesData(int newNumberOfUnlockedFeatures)
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
        }

        public List<ContainerHoldersData> GetContainerHoldersData() =>
            _gameDataProperty.Value?.containersData ?? new List<ContainerHoldersData>();

        public void SetContainersData(List<ContainerHoldersData> containersData)
        {
            if (containersData == null) return;
            _gameDataProperty.Value.containersData = containersData;
        }
        
        public List<ContainerHoldersData> ConvertToData(List<ContainerHolder> containerHolders)
        {
            List<ContainerHoldersData> data = GameDataProperty.Value.containersData;
            
            if (containerHolders.Count != data.Count)
            {
                return null;
            }

            for (int i = 0; i < containerHolders.Count; i++)
            {
                data[i].HasInitializedContainer = containerHolders[i].HasInitializedContainer;
                data[i].ParcelType = containerHolders[i].Type;
            }

            return data;
        }

        public async void LoadData(SerializableDataContainer dataContainer)
        {
            if (dataContainer.TryGet(GameDataKey, out GameData.GameData loadedData))
            {
                _gameDataProperty.Value = loadedData;
                if (_gameDataProperty.Value == null)
                    _gameDataProperty.Value = new GameData.GameData();
                await WaitForFixedUpdate();
                await _mediator.Send(new LoadDataCommand(_gameDataProperty.CurrentValue));
            }
        }

        public void SaveData(SerializableDataContainer dataContainer)
        {
            dataContainer.SetData(GameDataKey, _gameDataProperty.Value);
        }

        private async UniTask WaitForFixedUpdate() => 
            await UniTask.WaitForFixedUpdate();
    }
}