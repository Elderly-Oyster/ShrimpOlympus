using System.Collections.Generic;
using System.Net.NetworkInformation;
using CodeBase.Core.Systems;
using CodeBase.Core.Systems.Save;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.Game;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Cars;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Containers;
using R3;
using UnityEngine;
using VContainer;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers
{
    public class GameManager
    {
        private readonly ReceiverManager _receiverManager;
        private readonly ContainerManager _containerManager;
        private readonly LevelManager _levelManager;
        private readonly CarController _carController;
        private SaveSystem _saveSystem;
        private IAppEventService _appService;
        private readonly GameDataSystem _gameDataSystem;
        private readonly NPCCarManager _npcCarManager;
        
        private GameData _gameData;
        private CompositeDisposable _disposables = new();

        public GameManager(ReceiverManager receiverManager, ContainerManager containerManager, LevelManager levelManager, CarController carController, SaveSystem saveSystem, IAppEventService appService, GameDataSystem gameDataSystem, NPCCarManager npcCarManager)
        {
            _receiverManager = receiverManager;
            _containerManager = containerManager;
            _levelManager = levelManager;
            _carController = carController;
            _saveSystem = saveSystem;
            _appService = appService;
            _gameDataSystem = gameDataSystem;
            _npcCarManager = npcCarManager;
        }

        public void StartGame(float musicIsPlaying)
        {
            _gameData = _gameDataSystem.GameDataProperty.Value;
            _containerManager.Initialize(_gameData.containersData);
            _carController.SetMusicState(musicIsPlaying);
            _carController.Initialize(_gameData.capacity, _gameData.money);
            _levelManager.Initialize(_gameData.level, _gameData.experience, _gameData.numberOfUnlockedUpgrades);
            _receiverManager.Initialize(_gameData.containersData, _gameData.maxNumberOfOrders, musicIsPlaying);
            //_npcCarManager.Initialize();
            SubscribeToReactiveEvents();
        }

        public void EndGame()
        {
            _npcCarManager.Shutdown();
            Debug.Log("Game End, dispose");
            _disposables.Dispose();
        }

        private void SubscribeToReactiveEvents()
        {
            //Updates from managers and player for game data system 
            _disposables.Add(_carController.Money.
                Subscribe(money => _gameDataSystem.GameDataProperty.Value.money = money));
            _disposables.Add(_carController.Experience.Subscribe(experience => _levelManager.GetExperience(experience)));
            _disposables.Add(_levelManager.Level.
                Subscribe(level => _gameDataSystem.GameDataProperty.Value.level = level));
            _disposables.Add(_levelManager.Experience.
                Subscribe(experience => _gameDataSystem.GameDataProperty.Value.experience = experience));
            _disposables.Add(_levelManager.NumberOfUnlockedFeatures.
                Subscribe(numberOfUnlockedUpgrades => 
                    _gameDataSystem.GameDataProperty.Value.numberOfUnlockedUpgrades = numberOfUnlockedUpgrades));
            //Updates from popup
            _disposables.Add(_gameDataSystem.OnItemBought.Subscribe(money => _carController.UpdateMoney(money)));
            _disposables.Add(_gameDataSystem.OnContainerNeedsInitialization.
                    Subscribe(_ =>_containerManager.StartWarmUpOfContainer()));
            _disposables.Add(_gameDataSystem.OnUpdateCapacity.
                    Subscribe(capacity =>_carController.UpdateCarCapacity(capacity)));
            _disposables.Add(_gameDataSystem.OnUpdateMaxOrdersNumber.
                Subscribe(numberOfReceivers => _receiverManager.UpdateMaxNumberOfReceivers(numberOfReceivers)));
            //Updates from container manager 
            _disposables.Add(_containerManager.ContainerHoldersList.
                Subscribe(UpdateContainerHoldersData));
            _disposables.Add(_containerManager.ContainerHoldersList.
                Subscribe( data =>_receiverManager.UpdateReceiversTypes(data)));
        }

        private void UpdateContainerHoldersData(List<ContainerHolder> containerHolders)
        {
            List<ContainerHoldersData> data = _gameDataSystem.GameDataProperty.Value.containersData;
            
            if (containerHolders.Count != data.Count)
            {
                return;
            }

            for (int i = 0; i < containerHolders.Count; i++)
            {
                data[i].HasInitializedContainer = containerHolders[i].HasInitializedContainer;
                data[i].ParcelType = containerHolders[i].Type;
            }
            _gameDataSystem.SetContainersData(data);
        }
    }
}