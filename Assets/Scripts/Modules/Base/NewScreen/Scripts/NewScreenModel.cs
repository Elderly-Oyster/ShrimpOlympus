using Core;
using Cysharp.Threading.Tasks;

namespace Modules.Base.NewScreen.Scripts
{
    public class NewScreenModel : IScreenModel
    {
        private readonly IScreenController _rootController;
        private readonly NewScreenPresenter _ticTacScreenPresenter;

        public NewScreenModel(IScreenController rootController, NewScreenPresenter ticTacScreenPresenter)
        {
            _rootController = rootController;
            _ticTacScreenPresenter = ticTacScreenPresenter;
        }
        
        public async UniTask Run(object param)
        {
            _ticTacScreenPresenter.Initialize(this);
            await _ticTacScreenPresenter.ShowView();
            await _ticTacScreenPresenter.WaitForTransitionButtonPress();
        }

        public void RunMainMenuModel() => _rootController.RunModel(ScreenModelMap.MainMenu);
        
        public async UniTask Stop() => await _ticTacScreenPresenter.HideScreenView();
        public void Dispose() => _ticTacScreenPresenter.RemoveEventListeners();
    }
}