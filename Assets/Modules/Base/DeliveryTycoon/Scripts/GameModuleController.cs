using CodeBase.Core.Infrastructure;
using CodeBase.Core.Modules.MVP;
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
        private readonly GameScreenPresenter _gameScreenPresenter;
        private readonly UpgradePopupPresenter _upgradePopupPresenter;
        private readonly SplashScreenPresenter _splashScreenPresenter;
        private readonly PauseScreenPresenter _pauseScreenPresenter;
        private readonly GameModuleModel _gameModuleModel;
        private readonly InputSystemService _inputSystemService;
        private readonly UniTaskCompletionSource<bool> _moduleCompletionSource;
        private readonly CompositeDisposable _disposables = new();
        
        public GameModuleController(IScreenStateMachine screenStateMachine,
            GameModuleModel moduleModel, GameScreenPresenter gameScreenPresenter,
            UpgradePopupPresenter upgradePopupPresenter, SplashScreenPresenter splashScreenPresenter,
            PauseScreenPresenter pauseScreenPresenter, InputSystemService inputSystemService)
        {
            _screenStateMachine = screenStateMachine;
            _gameScreenPresenter = gameScreenPresenter;
            _upgradePopupPresenter = upgradePopupPresenter;
            _splashScreenPresenter = splashScreenPresenter;
            _pauseScreenPresenter = pauseScreenPresenter;
            _inputSystemService = inputSystemService;
            _gameModuleModel = moduleModel;
            SubscribeToEvents();
            _moduleCompletionSource = new UniTaskCompletionSource<bool>();
            _gameModuleModel.StateMachine.OnTransitionCompleted(OnStateChanged);
        }

        public async UniTask Enter(object param) => 
            await _gameModuleModel.ChangeState(GameModuleStates.Loading);

        public async UniTask Execute() => await _moduleCompletionSource.Task;

        public async UniTask Exit() => await _gameScreenPresenter.Exit();

        private void SubscribeToEvents()
        {
            _disposables.Add(_pauseScreenPresenter.OpenNewModuleCommand.Subscribe(RunNewScreen));
            _disposables.Add(_gameScreenPresenter.OnMainMenuButtonClickedCommand.Subscribe(RunNewScreen));
            _disposables.Add(_splashScreenPresenter.ServicesLoaded.
                Subscribe(async _ => await _gameModuleModel.ChangeState(GameModuleStates.Game)));
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
                    _gameScreenPresenter.HideStateInstantly();
                    _upgradePopupPresenter.HideStateInstantly();
                    _pauseScreenPresenter.HideStateInstantly();
                    
                    await UniTask.WhenAll(
                        _gameScreenPresenter.Enter(null),
                        _splashScreenPresenter.Enter(null)
                        );
                    break;
                }
                case GameModuleStates.Game:
                {
                    await UniTask.WhenAll(
                            _splashScreenPresenter.Exit(),
                            _upgradePopupPresenter.Exit(),
                            _pauseScreenPresenter.Exit()
                            );
                    await _gameScreenPresenter.ShowState();
                    _inputSystemService.SwitchToPlayerCar();
                    break;
                }

                case GameModuleStates.UpgradePopup:
                {
                    await _gameScreenPresenter.HideState();
                    await _upgradePopupPresenter.Enter(null);
                    _inputSystemService.SwitchToUI();
                    break;
                }

                case GameModuleStates.Pause:
                {
                    await _gameScreenPresenter.HideState();
                    await _pauseScreenPresenter.Enter(null);
                    _inputSystemService.SwitchToUI();
                    break;
                }
            }

            await UniTask.WaitForEndOfFrame();
            _gameModuleModel.ReleaseSemaphoreSlim();
        }

        private void RunNewScreen(ModulesMap screen)
        {
            _moduleCompletionSource.TrySetResult(true);
            _screenStateMachine.RunScreen(screen);
        }

        public void Dispose()
        {
            _gameScreenPresenter?.Dispose();
            _splashScreenPresenter?.Dispose();
            _gameModuleModel?.Dispose();
            _disposables.Dispose();
        }
    }
}