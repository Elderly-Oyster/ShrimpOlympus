using CodeBase.Core.Infrastructure;
using CodeBase.Core.Patterns.Architecture.MVP;
using Cysharp.Threading.Tasks;
using Modules.Additional.SplashScreen.Scripts;
using Modules.Base.DeliveryTycoon.Scripts.GameState;
using Modules.Base.DeliveryTycoon.Scripts.UpgradePopup;
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
        private readonly GameScreenModel _gameScreenModel;
        private readonly UniTaskCompletionSource<bool> _moduleCompletionSource;
        private readonly CompositeDisposable _disposables = new();
        
        public GameModuleController(IScreenStateMachine screenStateMachine,
            GameScreenModel screenModel, GameScreenPresenter gameScreenPresenter,
            UpgradePopupPresenter upgradePopupPresenter, SplashScreenPresenter splashScreenPresenter)
        {
            _screenStateMachine = screenStateMachine;
            _gameScreenPresenter = gameScreenPresenter;
            _upgradePopupPresenter = upgradePopupPresenter;
            _splashScreenPresenter = splashScreenPresenter;
            _gameScreenModel = screenModel;
            SubscribeToEvents();
            _moduleCompletionSource = new UniTaskCompletionSource<bool>();
            _gameScreenModel.StateMachine.OnTransitionCompleted(OnStateChanged);
        }

        public async UniTask Enter(object param)
        {
            await _gameScreenPresenter.Enter(null);
            await _splashScreenPresenter.Enter(null);
        }

        public async UniTask Execute() => await _moduleCompletionSource.Task;

        public async UniTask Exit() => await _gameScreenPresenter.Exit();

        private void SubscribeToEvents()
        {
            _disposables.Add(_gameScreenPresenter.OnMainMenuButtonClickedCommand.Subscribe(RunNewScreen));
            _disposables.Add(_splashScreenPresenter.ServicesLoaded.
                Subscribe(_ => _gameScreenModel.StateMachine.Fire(GameModuleStates.Game)));
        }

        private async void OnStateChanged(StateMachine<GameModuleStates, GameModuleStates>.Transition transition)
        {
            Debug.Log(transition.Destination);
            switch (transition.Destination)
            {
                case GameModuleStates.Loading:
                {
                    //TODO async refactoring
                    _gameScreenPresenter.ShowState();
                    break;
                }
                case GameModuleStates.Game:
                {
                    await _splashScreenPresenter.Exit();
                    await _upgradePopupPresenter.Exit();
                    _gameScreenPresenter.ShowState();
                    break;
                }

                case GameModuleStates.UpgradePopup:
                {
                    _gameScreenPresenter.HideState();
                    await _upgradePopupPresenter.Enter(null);
                    break;
                }
            }
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
            _gameScreenModel?.Dispose();
            _disposables.Dispose();
        }
    }
}