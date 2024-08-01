using Core;
using Cysharp.Threading.Tasks;

namespace Modules.Base.NewScreen.Scripts
{
    public class NewScreenModel : IScreenModel
    {
        private readonly IScreenController _rootController;
        private readonly NewScreenPresenter _ticTacScreenPresenter;
        
        private readonly UniTaskCompletionSource<bool> _completionSource;

        public NewScreenModel(IScreenController rootController,
            NewScreenPresenter ticTacScreenPresenter)
        {
            _completionSource = new UniTaskCompletionSource<bool>();
            _ticTacScreenPresenter = ticTacScreenPresenter;
            _rootController = rootController;
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
            _rootController.RunModel(ScreenModelMap.MainMenu);
        }

        public async UniTask Stop() => await _ticTacScreenPresenter.HideScreenView();
        public void Dispose() => _ticTacScreenPresenter.RemoveEventListeners();
    }
}