using Core;
using Core.MVVM;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Modules.Base.NewBaseScreen
{
    public class NewModuleScreenPresenter : IScreenPresenter
    {
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly NewModuleScreenModel _newModuleScreenModel;
        private readonly NewModuleScreenView _newModuleScreenView;
        private readonly UniTaskCompletionSource<bool> _completionSource;

        private readonly ReactiveCommand _mainMenuCommand = new ReactiveCommand();


        
        public NewModuleScreenPresenter(IScreenStateMachine screenStateMachine, 
            NewModuleScreenModel newModuleScreenModel, NewModuleScreenView newModuleScreenView)
        {
            _screenStateMachine = screenStateMachine;
            _newModuleScreenModel = newModuleScreenModel;
            _newModuleScreenView = newModuleScreenView;
            _completionSource = new UniTaskCompletionSource<bool>();
        }

        private void SubscribeToUIUpdates()
        {
            _mainMenuCommand.Subscribe(_ =>OnMainMenuButtonClicked());
        }

        public async UniTask Enter(object param)
        {
            _newModuleScreenView.gameObject.SetActive(false);
            SubscribeToUIUpdates();
            _newModuleScreenView.SetupEventListeners(
                _mainMenuCommand
            );
            await _newModuleScreenView.Show();
        }
        
        private void RunNewScreen(ScreenPresenterMap screen)
        {
            _completionSource.TrySetResult(true);
            _screenStateMachine.RunPresenter(screen);
        }

        private void OnMainMenuButtonClicked() => 
            RunNewScreen(ScreenPresenterMap.MainMenu);

        public async UniTask Execute() => await _completionSource.Task;

        public async UniTask Exit() => await _newModuleScreenView.Hide();

        public void Dispose()
        {
            _newModuleScreenView.Dispose();
            _newModuleScreenModel.Dispose();
        }
    }
}
