using CodeBase.Core.Infrastructure;
using CodeBase.Core.Modules;
using CodeBase.Core.Patterns.Architecture.MVP;
using Cysharp.Threading.Tasks;
using R3;

namespace Modules.Template.TemplateScreen.Scripts
{
    public class TemplateScreenPresenter : IScreenPresenter
    {
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly TemplateModuleModel _moduleModel;
        private readonly TemplateScreenView _screenView;
        private readonly UniTaskCompletionSource _screenCompletionSource;
        
        private readonly ReactiveCommand<Unit> _mainMenuCommand = new();
        
        public TemplateScreenPresenter(IScreenStateMachine screenStateMachine, 
            TemplateModuleModel moduleModel, TemplateScreenView screenView)
        {
            _screenStateMachine = screenStateMachine;
            _moduleModel = moduleModel;
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
        
        public async UniTask Exit() => await _screenView.Hide();

        public void Dispose()
        {
            _screenView.Dispose();
            _moduleModel.Dispose();
        }

        private void SubscribeToUIUpdates() => 
            _mainMenuCommand.Subscribe(_ =>OnMainMenuButtonClicked());

        private void OnMainMenuButtonClicked() => 
            RunNewScreen(ModulesMap.MainMenu);

        private void RunNewScreen(ModulesMap screen)
        {
            _screenCompletionSource.TrySetResult();
            _screenStateMachine.RunScreen(screen);
        }
    }
}
