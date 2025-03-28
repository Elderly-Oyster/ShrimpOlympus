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

        private const string GameDataMoneyKey = "GameDataMoney";
        private const string GameDataCapacityKey = "GameDataCapacity";
        private const string NumberOfUnlockedUpgradesKey = "NumberOfUnlockedUpgrades";
        private const string ExperienceKey = "Experience";
        private const string LevelKey = "Level";
        private const string MaxNumberOfOrdersKey = "MaxNumberOfOrders";
        private const string ContainerHolderDataKey = "ContainerHolderData";
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
                if (dataContainer.TryGet(GameDataMoneyKey, out GameData.GameData loadedGameData) && loadedGameData != null)
                    GameDataProperty.Value = loadedGameData;
                
                // if (dataContainer.TryGet(GameDataMoneyKey, 
                //         out int loadedMoneyData))
                //     GameDataProperty.Value.money = loadedMoneyData;
                //
                // if (dataContainer.TryGet(GameDataCapacityKey, 
                //         out int loadedCapacityData))
                //     GameDataProperty.Value.capacity = loadedCapacityData;
                //
                // if (dataContainer.TryGet(NumberOfUnlockedUpgradesKey, 
                //         out int unlockedUpgradesData))
                //     GameDataProperty.Value.numberOfUnlockedUpgrades = unlockedUpgradesData;
                //
                // if (dataContainer.TryGet(ExperienceKey, 
                //         out int  experienceData))
                //     GameDataProperty.Value.experience = experienceData;
                //
                // if (dataContainer.TryGet(LevelKey, out int levelData))
                //     GameDataProperty.Value.level = levelData;
                //
                // if (dataContainer.TryGet(MaxNumberOfOrdersKey, 
                //         out int  maxNumberOfOrdersData))
                //     GameDataProperty.Value.maxNumberOfOrders = maxNumberOfOrdersData;
                //
                // if (dataContainer.TryGet(ContainerHolderDataKey, 
                //         out List<ContainerHoldersData> containerHoldersData) && containerHoldersData != null)
                //     GameDataProperty.Value.containersData = containerHoldersData;

                _isLoaded= true;
                _dataLoaded.TrySetResult(true);
                Debug.Log("GameDataSystem received game data");
            }

            return UniTask.CompletedTask;
        }

        public void SaveData(SerializableDataContainer dataContainer)
        {
            Debug.Log("Number of saved containers: " + GameDataProperty.CurrentValue.containersData.Count);
            dataContainer.SetData(GameDataMoneyKey, GameDataProperty.CurrentValue);
            // dataContainer.SetData(GameDataCapacityKey, GameDataProperty.CurrentValue.capacity);
            // dataContainer.SetData(NumberOfUnlockedUpgradesKey, GameDataProperty.CurrentValue.numberOfUnlockedUpgrades);
            // dataContainer.SetData(ExperienceKey, GameDataProperty.CurrentValue.experience);
            // dataContainer.SetData(LevelKey, GameDataProperty.CurrentValue.level);
            // dataContainer.SetData(MaxNumberOfOrdersKey, GameDataProperty.CurrentValue.maxNumberOfOrders);
            // dataContainer.SetData(ContainerHolderDataKey, GameDataProperty.CurrentValue.containersData);
            //
        }
    }
}