using System.Collections.Generic;
using CodeBase.Core.Systems.Save;
using CodeBase.Core.Systems.Save.Game;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Cars;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Containers;
using VContainer;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers
{
    public class GameManager
    {
        [Inject] private ReceiverManager _receiverManager;
        [Inject] private ContainerManager _containerManager;
        [Inject] private LevelManager _levelManager;
        [Inject] private CarController _carController;
        [Inject] private SaveSystem _saveSystem;
        [Inject] private IAppService _appService;
        [Inject] private GameDataMediatorSystem _gameDataMediatorSystem;
        [Inject] private NPCCarManager _npcCarManager;
        
        private GameData _gameData;

        public async void StartGame(bool musicIsPlaying)
        {
            //await _saveSystem.LoadData();
            _gameData = _gameDataMediatorSystem.GameData;
            SubscribeToEvents();
            _containerManager.Initialize(_gameData.containersData);
            _carController.SetMusicState(musicIsPlaying);
            _carController.Initialize(_gameData.capacity, _gameData.money);
            _levelManager.Initialize(_gameData.level, _gameData.experience, _gameData.numberOfUnlockedUpgrades);
            _receiverManager.Initialize(_gameData.containersData, _gameData.maxNumberOfOrders, musicIsPlaying);
            _npcCarManager.Initialize();
        }

        public void EndGame()
        {
            SaveDataBeforeOpeningPopup();
            _npcCarManager.Shutdown();
            UnsubscribeFromEvents();
        }

        private void UpdateContainerHoldersData(List<ContainerHolder> containerHolders)
        {
            List<ContainerHoldersData> data = _gameData.containersData;
            
            if (containerHolders.Count != data.Count)
            {
                return;
            }

            for (int i = 0; i < containerHolders.Count; i++)
            {
                data[i].HasInitializedContainer = containerHolders[i].HasInitializedContainer;
                data[i].ParcelType = containerHolders[i].Type;
            }
            _gameDataMediatorSystem.SetContainersData(data);
        }

        private void SubscribeToEvents()
        {
            _gameDataMediatorSystem.OnUpdateCapacity += _carController.UpdateCarCapacity;
            _gameDataMediatorSystem.OnMoneyChanged += _carController.UpdateMoney;
            _carController.OnExperienceObtained += _levelManager.GetExperience;
            _containerManager.OnContainerAdded += UpdateContainerHoldersData;
            _containerManager.OnContainerAdded += _receiverManager.UpdateReceiversTypes;
            _gameDataMediatorSystem.OnContainerNeedsInitialization += _containerManager.StartWarmUpOfContainer;
            _gameDataMediatorSystem.OnUpdateMaxOrdersNumber += _receiverManager.UpdateMaxNumberOfReceivers;
        }

        private void UnsubscribeFromEvents()
        {
            _gameDataMediatorSystem.OnUpdateCapacity -= _carController.UpdateCarCapacity;
            _gameDataMediatorSystem.OnMoneyChanged -= _carController.UpdateMoney;
            _carController.OnExperienceObtained -= _levelManager.GetExperience;
            _containerManager.OnContainerAdded -= UpdateContainerHoldersData;
            _containerManager.OnContainerAdded -= _receiverManager.UpdateReceiversTypes;
            _gameDataMediatorSystem.OnContainerNeedsInitialization -= _containerManager.InitializeServiceBuilding;
            _gameDataMediatorSystem.OnUpdateMaxOrdersNumber -= _receiverManager.UpdateMaxNumberOfReceivers;
        }

        public void SaveDataBeforeOpeningPopup()
        {
            _gameDataMediatorSystem.GameData.level = _levelManager.Level;
            _gameDataMediatorSystem.GameData.experience = _levelManager.Experience;
            _gameDataMediatorSystem.GameData.capacity = _carController.Capacity;
            _gameDataMediatorSystem.GameData.money = _carController.Money;
            _gameDataMediatorSystem.GameData.maxNumberOfOrders = _receiverManager.MaxDemandingReceivers;
            _gameDataMediatorSystem.GameData.numberOfUnlockedUpgrades = _levelManager.NumberOfUnlockedFeatures;
        }
    }
}