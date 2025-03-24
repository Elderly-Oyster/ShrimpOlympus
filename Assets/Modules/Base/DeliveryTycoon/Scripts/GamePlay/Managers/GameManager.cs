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
        private readonly ReceiverManager _receiverManager;
        private readonly ContainerManager _containerManager;
        private readonly LevelService _levelService;
        private readonly CarController _player;
        private readonly NPCCarManager _npcCarManager;
        private readonly CurrencyService _currencyService;
        private readonly FakeManager _fakeManager;
        private GameData _gameData;
        
        public GameManager(ReceiverManager receiverManager,
            ContainerManager containerManager, 
            LevelService levelService, 
            CarController player,
            NPCCarManager npcCarManager,
            CurrencyService currencyService, FakeManager fakeManager)
        {
            _receiverManager = receiverManager;
            _containerManager = containerManager;
            _levelService = levelService;
            _player = player;
            _npcCarManager = npcCarManager;
            _currencyService = currencyService;
            _fakeManager = fakeManager;
        }

        public void SendGameData(GameData gameData)
        {
            _gameData = gameData;
            _player.Initialize(_gameData.capacity);
            _levelService.Initialize(_gameData.level, _gameData.experience, _gameData.numberOfUnlockedUpgrades);
            _receiverManager.Initialize(_gameData.containersData, _gameData.maxNumberOfOrders);
            _currencyService.Initialize(_gameData.money);
            _containerManager.Initialize(_gameData.containersData);
            _fakeManager.Initialize();
        }

        public void StartGame()
        {
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