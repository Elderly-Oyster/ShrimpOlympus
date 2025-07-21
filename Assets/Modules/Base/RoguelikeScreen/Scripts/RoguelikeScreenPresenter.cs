using CodeBase.Core.Infrastructure;
using CodeBase.Core.Infrastructure.Modules;
using Cysharp.Threading.Tasks;
using R3;

namespace Modules.Base.RoguelikeScreen.Scripts
{
    public class RoguelikeStatePresenter : IModuleController
    {
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly RoguelikeScreenModel _screenModel;
        private readonly RoguelikeScreenView _screenView;
        private readonly UniTaskCompletionSource _screenCompletionSource;
        
        private readonly ReactiveCommand<Unit> _mainMenuCommand = new();
        
        public RoguelikeStatePresenter(IScreenStateMachine screenStateMachine, 
            RoguelikeScreenModel screenModel, RoguelikeScreenView screenView)
        {
            _screenStateMachine = screenStateMachine;
            _screenModel = screenModel;
            _screenView = screenView;
            _screenCompletionSource = new UniTaskCompletionSource();
        }

        public async UniTask Enter(object param)
        {
            _screenView.HideInstantly();
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
            _mainMenuCommand.Subscribe(_ => OnMainMenuButtonClicked());

        private void OnMainMenuButtonClicked() => 
            RunNewScreen(ModulesMap.MainMenu);

        private void RunNewScreen(ModulesMap screen)
        {
            _screenCompletionSource.TrySetResult();
            _screenStateMachine.RunModule(screen);
        }
    }
}
