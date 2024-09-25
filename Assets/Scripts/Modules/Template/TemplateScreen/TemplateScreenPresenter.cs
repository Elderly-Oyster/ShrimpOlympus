using Core.MVP;
using Core.Root.ScreenStateMachine;
using Cysharp.Threading.Tasks;
using R3;

namespace Modules.Template.TemplateScreen
{
    public class TemplatePresenter : IScreenPresenter
    {
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly TemplateScreenModel _templateScreenModel;
        private readonly TemplateScreenView _templateScreenView;
        private readonly UniTaskCompletionSource<bool> _completionSource;

        private readonly ReactiveCommand<Unit> _mainMenuCommand = new();

        
        public TemplatePresenter(IScreenStateMachine screenStateMachine, 
            TemplateScreenModel templateScreenModel, TemplateScreenView templateScreenView)
        {
            _screenStateMachine = screenStateMachine;
            _templateScreenModel = templateScreenModel;
            _templateScreenView = templateScreenView;
            _completionSource = new UniTaskCompletionSource<bool>();
        }

        public async UniTask Enter(object param)
        {
            _templateScreenView.gameObject.SetActive(false);
            SubscribeToUIUpdates();
            _templateScreenView.SetupEventListeners(_mainMenuCommand);
            
            await _templateScreenView.Show();
        }
        
        private void SubscribeToUIUpdates() => 
            _mainMenuCommand.Subscribe(_ =>OnMainMenuButtonClicked());

        private void OnMainMenuButtonClicked() => 
            RunNewScreen(ScreenPresenterMap.MainMenu);
        
        private void RunNewScreen(ScreenPresenterMap screen)
        {
            _completionSource.TrySetResult(true);
            _screenStateMachine.RunScreen(screen);
        }

        public async UniTask Execute() => await _completionSource.Task;

        public async UniTask Exit() => await _templateScreenView.Hide();

        public void Dispose()
        {
            _templateScreenView.Dispose();
            _templateScreenModel.Dispose();
        }
    }
}
