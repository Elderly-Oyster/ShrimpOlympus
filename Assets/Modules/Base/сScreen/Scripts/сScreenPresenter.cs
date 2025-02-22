using CodeBase.Core.Infrastructure;
using CodeBase.Core.Modules;
using Cysharp.Threading.Tasks;
using R3;

namespace Modules.Base.сScreen.Scripts
{
    public class сPresenter : IScreenPresenter
    {
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly сScreenModel _screenModel;
        private readonly сScreenView _screenView;
        private readonly UniTaskCompletionSource _screenCompletionSource;
        
        private readonly ReactiveCommand<Unit> _mainMenuCommand = new();
        
        public сPresenter(IScreenStateMachine screenStateMachine, 
            сScreenModel screenModel, сScreenView screenView)
        {
            _screenStateMachine = screenStateMachine;
            _screenModel = screenModel;
            _screenView = screenView;
            _screenCompletionSource = new UniTaskCompletionSource();
        }

        public async UniTask Enter(object param)
        {
            _screenView.gameObject.SetActive(false);
            SubscribeToUIUpdates();
            _screenView.SetupEventListeners(_mainMenuCommand);
            
            await _screenView.Show();
        }

        public async UniTask Execute() => await _screenCompletionSource.Task;

        public async UniTask Exit() => await _screenView.Hide();

        public void Dispose()
        {
            _screenView.Dispose();
            _screenModel.Dispose();
        }

        private void SubscribeToUIUpdates() => 
            _mainMenuCommand.Subscribe(_ =>OnMainMenuButtonClicked());

        private void OnMainMenuButtonClicked() => 
            RunNewScreen(ScreenPresenterMap.MainMenu);

        private void RunNewScreen(ScreenPresenterMap screen)
        {
            _screenCompletionSource.TrySetResult();
            _screenStateMachine.RunScreen(screen);
        }
    }
}
