using System.Collections.Generic;
using System.Threading.Tasks;
using CodeBase.Core.Systems.Save;
using Cysharp.Threading.Tasks;
using MediatR;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameData;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Containers;
using R3;
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

        public readonly ReactiveCommand<Unit> OnContainerNeedsInitialization = new();
        public readonly ReactiveCommand<int> OnUpdateCapacity = new();
        public ReactiveProperty<GameData.GameData> GameDataProperty { get; private set; } = new();

        public ReactiveCommand<int> OnItemBought { get; private set; } = new();
        public readonly ReactiveCommand<int> OnUpdateMaxOrdersNumber = new();

        [Inject]
        public void Construct(SaveSystem saveSystem, Mediator mediator)
        {
            _saveSystem = saveSystem;
            _mediator = mediator;
            _saveSystem.AddSystem(this);
            GameDataProperty ??= new ReactiveProperty<GameData.GameData>(new GameData.GameData());
        }

        public int GetLevelData() => GameDataProperty.Value?.level ?? 0;
        public int GetMoneyData() => GameDataProperty.Value.money;

        // public int GetNumberOfUnlockedFeaturesData() => _gameDataProperty.Value.numberOfUnlockedUpgrades;
        //
        // public int GetCapacityData()=> _gameDataProperty.Value.capacity;
        //
        // public int GetMaxNumberOfOrdersData() => _gameDataProperty.Value.maxNumberOfOrders;

        public void InquireContainerInitialization() => OnContainerNeedsInitialization.Execute(Unit.Default);

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
            OnItemBought.Execute(newMoney);
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
            OnUpdateCapacity.Execute(newCapacity);
        }

        public void SetMaxNumberOfOrdersData(int newMaxNumberOfOrdersData)
        {
            if (newMaxNumberOfOrdersData < 0) return;
            GameDataProperty.Value.maxNumberOfOrders = newMaxNumberOfOrdersData;
            OnUpdateMaxOrdersNumber.Execute(newMaxNumberOfOrdersData);
        }

        public List<ContainerHoldersData> GetContainerHoldersData() =>
            GameDataProperty.Value?.containersData ?? new List<ContainerHoldersData>();

        public void SetContainersData(List<ContainerHoldersData> containersData)
        {
            if (containersData == null) return;
            GameDataProperty.Value.containersData = containersData;
        }
        
        public List<ContainerHoldersData> ConvertToData(List<ContainerHolder> containerHolders)
        {
            List<ContainerHoldersData> data = GameDataProperty.Value.containersData;
            
            if (containerHolders.Count != data.Count)
                return null;

            for (int i = 0; i < containerHolders.Count; i++)
            {
                data[i].hasInitializedContainer = containerHolders[i].HasInitializedContainer;
                data[i].parcelType = containerHolders[i].Type;
            }

            return data;
        }

        public async Task LoadData(SerializableDataContainer dataContainer)
        {
            if (dataContainer.TryGet(GameDataKey, out GameData.GameData loadedData))
            {
                GameDataProperty.Value = loadedData;
                if (GameDataProperty.Value == null)
                    GameDataProperty.Value = new GameData.GameData();
                await WaitForFixedUpdate(); // TODO 
                await _mediator.Send(new LoadDataCommand(GameDataProperty.CurrentValue));
            }
        }

        public void SaveData(SerializableDataContainer dataContainer) => 
            dataContainer.SetData(GameDataKey, GameDataProperty.Value);

        private static async UniTask WaitForFixedUpdate() => 
            await UniTask.WaitForFixedUpdate();
    }
}