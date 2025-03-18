using System.Threading.Tasks;
using CodeBase.Core.Infrastructure;
using CodeBase.Core.Modules;
using CodeBase.Core.Systems;
using CodeBase.Core.Systems.PopupHub;
using CodeBase.Core.Systems.Save;
using Cysharp.Threading.Tasks;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Cars;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers;
using R3;
using UnityEngine;

namespace Modules.Base.DeliveryTycoon.Scripts
{
    public class GameScreenPresenter : IScreenPresenter
    {
        private readonly LevelManager _levelManager;
        private readonly GameScreenModel _screenModel;
        private readonly GameScreenView _screenView;
        private readonly TaskCompletionSource<bool> _screenCompletionSource;
        private readonly CarController _carController;
        private readonly IPopupHub _popupHub;
        private readonly GameManager _gameManager;
        private readonly GameDataSystem _gameDataSystem;
        private readonly AudioSystem _audioSystem;
        private readonly SaveSystem _saveSystem;
        private CompositeDisposable _disposables = new();
        
        private readonly ReactiveCommand<ScreenPresenterMap> _onMainMenuButtonClicked = new();
        public ReactiveCommand<ScreenPresenterMap> OnMainMenuButtonClickedCommand => _onMainMenuButtonClicked;
        
        public GameScreenPresenter( GameScreenModel screenModel, 
            GameScreenView screenView, LevelManager levelManager, CarController carController,
            AudioSystem audioSystem, GameDataSystem gameDataSystem, GameManager gameManager, SaveSystem saveSystem, IPopupHub popupHub )
        {
            _popupHub = popupHub;
            _screenModel = screenModel;
            _screenView = screenView;
            _levelManager = levelManager;
            _carController = carController;
            _audioSystem = audioSystem;
            _gameDataSystem = gameDataSystem;
            _gameManager = gameManager;
            _saveSystem = saveSystem;
            _screenCompletionSource = new TaskCompletionSource<bool>();
        }

        public async UniTask Enter(object param)
        {
            _screenView.HideInstantly();
            _levelManager.OnUpdateViewProgressBar += UpdateExperienceProgressBar;
            SubscribeToReactiveEvents();
            var musicVolume = _audioSystem.MusicVolume;
            if (musicVolume > 0)
            {
                Debug.Log("Background Music is playing");
                _audioSystem.PlayGameMelody();
            }
            _gameManager.StartGame(musicVolume);
            
            _screenView.SetupEventListeners
            (
                OnMainMenuButtonClicked,
                OnUpgradePopupButtonClicked
                
            );
            
            await _screenView.Show();
            _screenView.InitializeVisualElements(_gameDataSystem.GetMoneyData(), _gameDataSystem.GetLevelData());
        }

        public async UniTask Execute() => await _screenCompletionSource.Task;

        public async UniTask Exit()
        {
            _saveSystem.SaveData().Forget();
            _screenCompletionSource.TrySetResult(true);
            await _screenView.Hide();
        }

        public async void ShowGameScreenView()
        {
            await _screenView.Show();
        }
        private void SubscribeToReactiveEvents()
        {
            _disposables.Add(_carController.Money.Subscribe(UpdateMoneyCounter));
            _disposables.Add(_levelManager.Level.Subscribe(UpdateLevelText));
        }
        
        private void UpdateMoneyCounter(int money) => _screenView.UpdatePlayerMoney(money);
        
        private void UpdateLevelText(int level) => _screenView.UpdateLevel(level);
        
        private void UpdateExperienceProgressBar(float experience) => _screenView.UpdateExperience(experience);


        private void OnMainMenuButtonClicked() => 
            RunNewScreen(ScreenPresenterMap.MainMenu);

        private async void OnUpgradePopupButtonClicked()
        {
            _screenModel.ChangeState(GameScreenState.UpgradePopup);
            await _screenView.Hide();
        }

        private void RunNewScreen(ScreenPresenterMap screen)
        {
             //await _saveSystem.SaveData();
           _onMainMenuButtonClicked.Execute(screen);
        }

        public void Dispose()
        {
            _levelManager.OnUpdateViewProgressBar -= UpdateExperienceProgressBar;
            _gameManager.EndGame();
            _screenView.Dispose();
            _screenModel.Dispose();
        }
    }
}
