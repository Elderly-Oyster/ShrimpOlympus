using CodeBase.Core.Infrastructure;
using CodeBase.Core.Modules;
using Cysharp.Threading.Tasks;
using R3;

namespace Modules.Base.NewModuleScreen.Scripts
{
    public class NewModulePresenter : IScreenPresenter
    {
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly NewModuleScreenModel _screenModel;
        private readonly NewModuleScreenView _screenView;
        private readonly UniTaskCompletionSource _screenCompletionSource;
        
        private readonly ReactiveCommand<Unit> _mainMenuCommand = new();
        
        public NewModulePresenter(IScreenStateMachine screenStateMachine, 
            NewModuleScreenModel screenModel, NewModuleScreenView screenView)
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
