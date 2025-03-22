using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameData;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Cars.Player;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.CurrencyService;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.LevelService;
using R3;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers
{
    public class GameManager
    {
        private readonly ReceiverManager _receiverManager;
        private readonly ContainerManager.ContainerManager _containerManager;
        private readonly LevelService _levelService;
        private readonly CarController _player;
        private readonly NPCCarManager _npcCarManager;
        private readonly CurrencyService _currencyService;
        private float _musicVolume;
        private GameData _gameData;
        private CompositeDisposable _disposables = new();

        public GameManager(ReceiverManager receiverManager,
            ContainerManager.ContainerManager containerManager, LevelService levelService, 
            CarController player, NPCCarManager npcCarManager, CurrencyService currencyService)
        {
            _receiverManager = receiverManager;
            _containerManager = containerManager;
            _levelService = levelService;
            _player = player;
            _npcCarManager = npcCarManager;
            _currencyService = currencyService;
        }

        public float MusicVolume => _musicVolume;

        public void SetMusicData(float musicVolume) => _musicVolume = musicVolume;

        public void StartGame(float musicIsPlaying, GameData gameData)
        {
            _gameData = gameData;
            _containerManager.Initialize(_gameData.containersData);
            _player.SetMusicState(musicIsPlaying);
            _player.Initialize(_gameData.capacity);
            _levelService.Initialize(_gameData.level, _gameData.experience, _gameData.numberOfUnlockedUpgrades);
            _receiverManager.Initialize(_gameData.containersData, _gameData.maxNumberOfOrders, musicIsPlaying);
            _currencyService.Initialize(_gameData.money);
            _npcCarManager.Initialize();
        }

        public void EndGame()
        {
            //_npcCarManager.Shutdown();
            _disposables.Dispose();
        }
    }
}