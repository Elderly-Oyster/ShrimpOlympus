using Core;
using Core.MVVM;
using Cysharp.Threading.Tasks;

namespace Modules.Base.NewScreen.Scripts
{
    public class NewScreenModel : IScreenModel
    {
        private readonly IScreenStateMachine _rootStateMachine;
        private readonly NewScreenPresenter _ticTacScreenPresenter;
        
        private readonly UniTaskCompletionSource<bool> _completionSource;

        public NewScreenModel(IScreenStateMachine rootStateMachine,
            NewScreenPresenter ticTacScreenPresenter)
        {
            _completionSource = new UniTaskCompletionSource<bool>();
            _ticTacScreenPresenter = ticTacScreenPresenter;
            _rootStateMachine = rootStateMachine;
        }
        
        public async UniTask Run(object param)
        {
            _ticTacScreenPresenter.Initialize(this);
            await _ticTacScreenPresenter.ShowView();
            await _completionSource.Task;
        }

        public void RunMainMenuModel()
        {
            _completionSource.TrySetResult(true);
            _rootStateMachine.RunPresenter(ScreenPresenterMap.MainMenu);
        }

        public async UniTask Stop() => await _ticTacScreenPresenter.HideScreenView();
        public void Dispose() => _ticTacScreenPresenter.RemoveEventListeners();
    }
}