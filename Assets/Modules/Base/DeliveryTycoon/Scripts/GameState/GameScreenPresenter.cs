using System.Threading.Tasks;
using CodeBase.Core.Infrastructure;
using CodeBase.Core.Modules;
using CodeBase.Core.Systems;
using CodeBase.Core.Systems.Save;
using CodeBase.Services;
using Cysharp.Threading.Tasks;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameDataSystem;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.CurrencyService;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.LevelService;
using R3;

namespace Modules.Base.DeliveryTycoon.Scripts.GameState
{
    public class GameScreenPresenter : IScreenPresenter
    {
        private readonly GameScreenModel _screenModel;
        private readonly GameView _view;
        private readonly CurrencyService _currencyService;
        private readonly LevelService _levelService;
        private readonly GameDataSystem _gameDataSystem;
        private readonly AudioSystem _audioSystem;
        private readonly SaveSystem _saveSystem;
        private readonly GameManager _gameManager;
        private readonly LoadingServiceProvider _loadingServiceProvider;
        private readonly TaskCompletionSource<bool> _screenCompletionSource;

        private readonly ReactiveCommand<ModulesMap> _onMainMenuButtonClicked = new();
        private readonly ReactiveCommand<Unit> _onMainMenuButtonClickedCommand = new();
        private readonly ReactiveCommand<Unit> _onUpgradePopupButtonClickedCommand = new();
        private readonly ReactiveCommand<Unit> _pausePopupCommand = new();
        
        private CompositeDisposable _disposables = new();
        
        public ReactiveCommand<ModulesMap> OnMainMenuButtonClickedCommand => _onMainMenuButtonClicked;

        public GameScreenPresenter( GameScreenModel screenModel, GameView view, LevelService levelService,
            AudioSystem audioSystem, GameDataSystem gameDataSystem, GameManager gameManager, SaveSystem saveSystem,
            CurrencyService currencyService, LoadingServiceProvider loadingServiceProvider)
        {
            _screenModel = screenModel;
            _view = view;
            _levelService = levelService;
            _currencyService = currencyService;
            _loadingServiceProvider = loadingServiceProvider;
            _audioSystem = audioSystem;
            _gameDataSystem = gameDataSystem;
            _gameManager = gameManager;
            _saveSystem = saveSystem;
            _screenCompletionSource = new TaskCompletionSource<bool>();
            
            SubscribeToUIUpdates();
        }

        public async UniTask Enter(object param)
        {
            _view.HideInstantly();
            
            await _gameDataSystem.DataLoaded.Task;
            _gameManager.SendGameData(_gameDataSystem.GameDataProperty.CurrentValue);
            _loadingServiceProvider.CompleteRegistration();
            _gameManager.StartGame();
            SubscribeToReactiveEvents();
            
            var musicVolume = _audioSystem.MusicVolume;
            if (musicVolume > 0) 
                _audioSystem.PlayGameMelody();
            
            _view.SetupEventListeners
            (
               //_onMainMenuButtonClickedCommand,
               _onUpgradePopupButtonClickedCommand, 
               _pausePopupCommand
            );
            
            _view.InitializeVisualElements(_gameDataSystem.GetMoneyData(), _gameDataSystem.GetLevelData());
            _loadingServiceProvider.ResetRegistrationProgress();
        }

        public async UniTask Execute() => await _screenCompletionSource.Task;

        public async UniTask Exit()
        {
            _gameManager.EndGame();
            _saveSystem.SaveData().Forget();
            _screenCompletionSource.TrySetResult(true);
            //await _view.Hide();
        }

        public async UniTask ShowState()
        {
            await _view.Show();
            //other hide sub-state logic
        }

        public async UniTask HideState()
        {
            await _view.Hide();
            //other hide sub-state logic
        }
        
        public void HideStateInstantly() => _view.HideInstantly();

        private void SubscribeToReactiveEvents()
        {
            _disposables.Add(_currencyService.Money.Subscribe(UpdateMoneyCounter));
            _disposables.Add(_levelService.Level.Subscribe(UpdateLevelText));
            _disposables.Add(_levelService.ExperienceForProgressBar.Subscribe(UpdateExperienceProgressBar));
        }

        private void SubscribeToUIUpdates()
        {
            //_onMainMenuButtonClickedCommand.Subscribe(_ => OnMainMenuButtonClicked());
            _onUpgradePopupButtonClickedCommand.Subscribe(async _ => await OnUpgradePopupButtonClicked());
            _pausePopupCommand.Subscribe(async _ => await OnPausePopupButtonClicked());
            
        }
        
        private void UpdateMoneyCounter(int money) => _view.UpdatePlayerMoney(money);
        
        private void UpdateLevelText(int level) => _view.UpdateLevel(level);
        
        private void UpdateExperienceProgressBar(float experience) => 
            _view.UpdateExperience(experience);

        private void OnMainMenuButtonClicked() => 
            _onMainMenuButtonClicked.Execute(ModulesMap.MainMenu);

        private async UniTask OnPausePopupButtonClicked() => await _screenModel.ChangeState(GameModuleStates.Pause);

        private async UniTask OnUpgradePopupButtonClicked()
        {
            await _screenModel.ChangeState(GameModuleStates.UpgradePopup);
        }
        
        public void Dispose()
        {
            _view.Dispose();
            _screenModel.Dispose();
        }
    }
}
