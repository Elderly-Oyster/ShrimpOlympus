using CodeBase.Core.Infrastructure;
using CodeBase.Core.Patterns.Architecture.MVP;
using Cysharp.Threading.Tasks;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameDataSystemLogic;
using Modules.Base.DeliveryTycoon.Scripts.UpgradePopupLogic;
using R3;
using Stateless;
using UnityEngine;

namespace Modules.Base.DeliveryTycoon.Scripts
{
    public class GameScreenController : IStateController
    {
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly GameScreenPresenter _gameScreenPresenter;
        private readonly UpgradePopupPresenter _upgradePopupPresenter;
        private readonly GameScreenModel _gameScreenModel;
        private readonly UniTaskCompletionSource<bool> _completionSource;
        private readonly CompositeDisposable _disposables = new();
        
        
        public GameScreenController(IScreenStateMachine screenStateMachine,
            GameScreenModel screenModel, GameScreenPresenter gameScreenPresenter, UpgradePopupPresenter upgradePopupPresenter, GameDataSystem gameDataSystem)
        {
            _screenStateMachine = screenStateMachine;
            _gameScreenPresenter = gameScreenPresenter;
            _upgradePopupPresenter = upgradePopupPresenter;
            _gameScreenModel = screenModel;
            _gameScreenModel = screenModel;
            SubscribeToEvents();
            _completionSource = new UniTaskCompletionSource<bool>();
            _gameScreenModel.StateMachine.OnTransitionCompleted(OnStateChanged);
        }

        private void SubscribeToEvents()
        {
            _disposables.Add(_gameScreenPresenter.OnMainMenuButtonClickedCommand.Subscribe(RunNewScreen));
        }

        public async UniTask Enter(object param)
        {
             await _gameScreenPresenter.Enter(null);
        }

        public async UniTask Execute() => await _completionSource.Task;

        public async UniTask Exit() => await _gameScreenPresenter.Exit();
        
        private async void OnStateChanged(StateMachine<GameScreenState, GameScreenState>.Transition transition)
        {
            Debug.Log(transition.Destination);
            switch (transition.Destination)
            {
                case GameScreenState.Game:
                {
                    await _upgradePopupPresenter.Exit();
                    _gameScreenPresenter.ShowGameScreenView();
                    break;
                }

                case GameScreenState.UpgradePopup:
                {
                    await _upgradePopupPresenter.Enter(null);
                    break;
                }
            }
        }

        private void RunNewScreen(ScreenPresenterMap screen)
        {
            _completionSource.TrySetResult(true);
            _screenStateMachine.RunScreen(screen);
        }

        public void Dispose()
        {
            _gameScreenPresenter?.Dispose();
            _gameScreenModel?.Dispose();
            _disposables.Dispose();
        }
    }
}