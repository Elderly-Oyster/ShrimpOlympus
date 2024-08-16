using Core.MVVM;
using Cysharp.Threading.Tasks;

namespace Modules.Base.NewBaseScreen
{
    public class NewModuleScreenViewModel : IScreenViewModel
    {
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly NewModuleScreenModel _newModuleScreenModel;
        private readonly NewModuleScreenView _newModuleScreenView;
        private readonly UniTaskCompletionSource<bool> _completionSource;
        
        public NewModuleScreenViewModel(IScreenStateMachine screenStateMachine, 
            NewModuleScreenModel newModuleScreenModel, NewModuleScreenView newModuleScreenView)
        {
            _screenStateMachine = screenStateMachine;
            _newModuleScreenModel = newModuleScreenModel;
            _newModuleScreenView = newModuleScreenView;
            _completionSource = new UniTaskCompletionSource<bool>();
        }
        
        public async UniTask Run(object param)
        {
            _newModuleScreenView.gameObject.SetActive(false);
            _newModuleScreenView.SetupEventListeners
            (
                
            );
            await _newModuleScreenView.Show();
            await _completionSource.Task;
        }

        public async UniTask Stop() => await _newModuleScreenView.Hide();

        public void Dispose()
        {
            //If model is used, there must be removing event listeners of model
            _newModuleScreenView.Dispose();
            _newModuleScreenModel.Dispose();
        }

        private void RunNewScreen(ScreenViewModelMap screen)
        {
            _completionSource.TrySetResult(true);
            _screenStateMachine.RunViewModel(screen);
        }
    }
}