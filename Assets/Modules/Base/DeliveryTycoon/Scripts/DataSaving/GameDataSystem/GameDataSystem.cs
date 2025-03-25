using System.Collections.Generic;
using System.Threading.Tasks;
using CodeBase.Core.Systems.Save;
using Cysharp.Threading.Tasks;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameData;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Containers;
using R3;
using UnityEngine;
using VContainer;

namespace Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameDataSystem
{
    public class GameDataSystem : ISerializableDataSystem
    {
        private SaveSystem _saveSystem;

        private const string GameDataKey = "GameData";
        private readonly TaskCompletionSource<bool> _dataLoaded = new();
        private bool _isLoaded;
        
        public ReactiveProperty<GameData.GameData> GameDataProperty { get; private set; } = new();

        public TaskCompletionSource<bool> DataLoaded => _dataLoaded;

        [Inject]
        public void Construct(SaveSystem saveSystem)
        {
            GameDataProperty ??= new ReactiveProperty<GameData.GameData>();
            GameDataProperty.Value ??= new GameData.GameData();
            _saveSystem = saveSystem;
            _saveSystem.AddSystem(this);
            // _dataLoaded = new TaskCompletionSource<bool>();
        }

        public int GetLevelData() => GameDataProperty.Value?.level ?? 0;
        public int GetMoneyData() => GameDataProperty.Value.money;

        public void SetLevelData(int newLevel)
        {
            if (newLevel < 0) return;
            GameDataProperty.Value.level = newLevel;
        }

        public void SetExperience(int newExperience)
        {
            if (newExperience < 0) return;
            GameDataProperty.Value.experience = newExperience;
        }

        public void SetMoneyData(int newMoney)
        {
            if (newMoney < 0) return;
            GameDataProperty.Value.money = newMoney;
        }

        public void SetNumberOfUnlockedUpgradesData(int newNumberOfUnlockedFeatures)
        {
            if (newNumberOfUnlockedFeatures < 0) return;
            GameDataProperty.Value.numberOfUnlockedUpgrades = newNumberOfUnlockedFeatures;
        }

        public void SetCapacityData(int newCapacity)
        {
            if (newCapacity < 0) return;    
            GameDataProperty.Value.capacity = newCapacity;
        }

        public void SetMaxNumberOfOrdersData(int newMaxNumberOfOrdersData)
        {
            if (newMaxNumberOfOrdersData < 0) return;
            GameDataProperty.Value.maxNumberOfOrders = newMaxNumberOfOrdersData;
        }

        public void SetContainersData(List<ContainerHoldersData> containersData)
        {
            if (containersData == null) return;
            GameDataProperty.Value.containersData = new List<ContainerHoldersData>(containersData);
           
        }
        
        public List<ContainerHoldersData> ConvertToData(List<ContainerHolder> containerHolders)
        {
            List<ContainerHoldersData> data = GameDataProperty.CurrentValue.containersData;
            
            if (containerHolders.Count != data.Count)
                return null;

            for (int i = 0; i <  GameDataProperty.CurrentValue.containersData.Count; i++)
            {
                data[i].hasInitializedContainer = containerHolders[i].HasInitializedContainer;
                data[i].parcelType = containerHolders[i].Type;
            }
            
            Debug.Log("Convert To Data counter: " + data.Count);

            return data;
        }

        public UniTask LoadData(SerializableDataContainer dataContainer)
        {
            if (!_isLoaded)
            {
                if (dataContainer.TryGet(GameDataKey, out GameData.GameData loadedData) && loadedData != null)
                {
                    //TODO the data is not overwritten properly, the list of containerHolderData is added to the existing one
                    GameDataProperty.CurrentValue.containersData.Clear();
                    GameDataProperty.Value = loadedData;
                    GameDataProperty.ForceNotify();
                }

                _isLoaded = true;
                _dataLoaded.TrySetResult(true);
                Debug.Log("GameDataSystem received game data");
            }

            return UniTask.CompletedTask;
        }

        public void SaveData(SerializableDataContainer dataContainer)
        {
            Debug.Log("Number of saved containers: " + GameDataProperty.CurrentValue.containersData.Count);
            dataContainer.SetData(GameDataKey, GameDataProperty.CurrentValue);
        }
    }
}