using Cysharp.Threading.Tasks;
using MVP.MVP_Root_Model.Scripts.Core;

namespace MVP.MVP_Root_Model.Scripts.Modules.TicTacScreen.Scripts
{
    public class TicTacScreenModel : IScreenModel
    {
        private readonly IRootController _rootController;
        private readonly TicTacScreenPresenter _ticTacScreenPresenter;

        public TicTacScreenModel(IRootController rootController, TicTacScreenPresenter ticTacScreenPresenter)
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