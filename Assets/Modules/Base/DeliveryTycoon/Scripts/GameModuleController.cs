using CodeBase.Core.Infrastructure;
using CodeBase.Core.Infrastructure.Modules;
using CodeBase.Core.Patterns.Architecture.MVP;
using CodeBase.Services;
using Cysharp.Threading.Tasks;
using Modules.Additional.SplashScreen.Scripts;
using Modules.Base.DeliveryTycoon.Scripts.GameState;
using Modules.Base.DeliveryTycoon.Scripts.PausePopup;
using Modules.Base.DeliveryTycoon.Scripts.UpgradePopupState;
using R3;
using Stateless;
using UnityEngine;

namespace Modules.Base.DeliveryTycoon.Scripts
{
    public class GameModuleController : IModuleController
    {
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly GamePresenter _gamePresenter;
        private readonly UpgradePopupPresenter _upgradePopupPresenter;
        private readonly SplashPresenter _splashPresenter;
        private readonly PausePresenter _pausePresenter;
        private readonly GameModel _gameModel;
        private readonly InputSystemService _inputSystemService;
        private readonly UniTaskCompletionSource<bool> _moduleCompletionSource;
        private readonly CompositeDisposable _disposables = new();
        
        public GameModuleController(IScreenStateMachine screenStateMachine,
            GameModel model, GamePresenter gamePresenter,
            UpgradePopupPresenter upgradePopupPresenter, SplashPresenter splashPresenter,
            PausePresenter pausePresenter, InputSystemService inputSystemService)
        {
            _screenStateMachine = screenStateMachine;
            _gamePresenter = gamePresenter;
            _upgradePopupPresenter = upgradePopupPresenter;
            _splashPresenter = splashPresenter;
            _pausePresenter = pausePresenter;
            _inputSystemService = inputSystemService;
            _gameModel = model;
            SubscribeToEvents();
            _moduleCompletionSource = new UniTaskCompletionSource<bool>();
            _gameModel.StateMachine.OnTransitionCompleted(OnStateChanged);
        }

        public async UniTask Enter(object param) => 
            await _gameModel.ChangeState(GameModuleStates.Loading);

        public async UniTask Execute() => await _moduleCompletionSource.Task;

        public async UniTask Exit() => await _gamePresenter.Exit();

        private void SubscribeToEvents()
        {
            _disposables.Add(_pausePresenter.OpenNewModuleCommand.Subscribe(RunNewScreen));
            _disposables.Add(_gamePresenter.OnMainMenuButtonClickedCommand.Subscribe(RunNewScreen));
            _disposables.Add(_splashPresenter.ServicesLoaded.
                Subscribe(async _ => await _gameModel.ChangeState(GameModuleStates.Game)));
        }
        
        private async void OnStateChanged(StateMachine<GameModuleStates, GameModuleStates>.Transition transition) => 
            await HandleStateTransitionAsync(transition);

        private async UniTask HandleStateTransitionAsync(StateMachine<GameModuleStates, GameModuleStates>.Transition transition)
        {
            Debug.Log(transition.Destination);
            _inputSystemService.InputActions.Disable();
            switch (transition.Destination)
            {
                case GameModuleStates.Loading:
                {
                    //TODO async refactoring
                    _gamePresenter.HideStateInstantly();
                    _upgradePopupPresenter.HideStateInstantly();
                    _pausePresenter.HideStateInstantly();
                    
                    await UniTask.WhenAll(
                        _gamePresenter.Enter(null),
                        _splashPresenter.Enter(null)
                        );
                    break;
                }
                case GameModuleStates.Game:
                {
                    await UniTask.WhenAll(
                            _splashPresenter.Exit(),
                            _upgradePopupPresenter.Exit(),
                            _pausePresenter.Exit()
                            );
                    await _gamePresenter.ShowState();
                    _inputSystemService.SwitchToPlayerCar();
                    break;
                }

                case GameModuleStates.UpgradePopup:
                {
                    await _gamePresenter.HideState();
                    await _upgradePopupPresenter.Enter(null);
                    _inputSystemService.SwitchToUI();
                    break;
                }

                case GameModuleStates.Pause:
                {
                    await _gamePresenter.HideState();
                    await _pausePresenter.Enter(null);
                    _inputSystemService.SwitchToUI();
                    break;
                }
            }

            await UniTask.WaitForEndOfFrame();
            _gameModel.ReleaseSemaphoreSlim();
        }

        private void RunNewScreen(ModulesMap screen)
        {
            _moduleCompletionSource.TrySetResult(true);
            _screenStateMachine.RunModule(screen);
        }

        public void Dispose()
        {
            _gamePresenter?.Dispose();
            _splashPresenter?.Dispose();
            _gameModel?.Dispose();
            _disposables.Dispose();
        }
    }
}