using Core;
using Core.MVVM;
using Cysharp.Threading.Tasks;

namespace Modules.Base.NewBaseScreen
{
    public class NewModuleScreenPresenter : IScreenPresenter
    {
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly NewModuleScreenModel _newModuleScreenModel;
        private readonly NewModuleScreenView _newModuleScreenView;
        private readonly UniTaskCompletionSource<bool> _completionSource;
        
        public NewModuleScreenPresenter(IScreenStateMachine screenStateMachine, 
            NewModuleScreenModel newModuleScreenModel, NewModuleScreenView newModuleScreenView)
        {
            _screenStateMachine = screenStateMachine;
            _newModuleScreenModel = newModuleScreenModel;
            _newModuleScreenView = newModuleScreenView;
            _completionSource = new UniTaskCompletionSource<bool>();
        }
        
        public async UniTask Enter(object param)
        {
            _newModuleScreenView.gameObject.SetActive(false);
            _newModuleScreenView.SetupEventListeners
            (
                //Here send UnityActions(Methods from this class) to view
            );
            await _newModuleScreenView.Show();
        }

        public async UniTask Execute() => await _completionSource.Task;

        public async UniTask Exit() => await _newModuleScreenView.Hide();

        public void Dispose()
        {
            //If model is used, there must be removing event listeners of model
            _newModuleScreenView.Dispose();
            _newModuleScreenModel.Dispose();
        }

        private void RunNewScreen(ScreenPresenterMap screen)
        {
            _completionSource.TrySetResult(true);
            _screenStateMachine.RunPresenter(screen);
        }
    }
}