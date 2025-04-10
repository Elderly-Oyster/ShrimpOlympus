using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeBase.Services;
using Cysharp.Threading.Tasks;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameData;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Cars.Player;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers.ContainerManagerLogic;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.CurrencyService;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.LevelService;
using UnityEngine;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers
{
    public class GameManager
    {
        private readonly InputSystemService _inputSystemService;
        private readonly CurrencyService _currencyService;
        private readonly LevelService _levelService;
        private readonly ContainerManager _containerManager;
        private readonly ReceiverManager _receiverManager;
        private readonly NPCCarManager _npcCarManager;
        private readonly FakeManager _fakeManager;
        private readonly CarController _player;
        private GameData _gameData;
        
        public GameManager(InputSystemService inputSystemService, CurrencyService currencyService, LevelService levelService, 
            ContainerManager containerManager, ReceiverManager receiverManager, NPCCarManager npcCarManager, 
            FakeManager fakeManager, CarController player)
        {
            _inputSystemService = inputSystemService;
            _currencyService = currencyService;
            _levelService = levelService;
            _containerManager = containerManager;
            _receiverManager = receiverManager;
            _npcCarManager = npcCarManager;
            _fakeManager = fakeManager;
            _player = player;
        }

        public void SendGameData(GameData gameData)
        {
            _gameData = gameData;
            _currencyService.Initialize(_gameData.money);
            _levelService.Initialize(_gameData.level, _gameData.experience, _gameData.numberOfUnlockedUpgrades);
            _containerManager.Initialize(_gameData.containersData);
            _receiverManager.Initialize(_gameData.containersData, _gameData.maxNumberOfOrders);
            _fakeManager.Initialize();
            _player.Initialize(_gameData.capacity);
        }

        public void StartGame()
        {
            _inputSystemService.SwitchToPlayerCar();
            _receiverManager.StartAssignReceivers();
            _npcCarManager.StartSpawnCars();
        }

        public void EndGame()
        {
            _containerManager.ResetParameters();
            _fakeManager.ResetParameters();
        }
    }
}