using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameData;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Cars.Player;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers.ContainerManagerLogic;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.CurrencyServiceLogic;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.LevelServiceLogic;

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
        private TycoonData _tycoonData;

        public float MusicVolume { get; private set; }
        
        public GameManager(ReceiverManager receiverManager,
            ContainerManager containerManager, 
            LevelService levelService, 
            CarController player,
            NPCCarManager npcCarManager,
            CurrencyService currencyService)
        {
            _receiverManager = receiverManager;
            _containerManager = containerManager;
            _levelService = levelService;
            _player = player;
            _npcCarManager = npcCarManager;
            _currencyService = currencyService;
        }
        
        public void SetMusicData(float musicVolume) => MusicVolume = musicVolume;

        public void StartGame(float musicIsPlaying, TycoonData tycoonData)
        {
            _tycoonData = tycoonData;
            _containerManager.Initialize(_tycoonData.containersData);
            _player.SetMusicState(musicIsPlaying);  //TODO remove this and inject audio service to the class
            _player.Initialize(_tycoonData.capacity);
            _levelService.Initialize(_tycoonData.level, _tycoonData.experience, _tycoonData.numberOfUnlockedUpgrades);
            _receiverManager.Initialize(_tycoonData.containersData, _tycoonData.maxNumberOfOrders, musicIsPlaying); //
            _currencyService.Initialize(_tycoonData.money);
            _npcCarManager.Initialize();
        }
    }
}