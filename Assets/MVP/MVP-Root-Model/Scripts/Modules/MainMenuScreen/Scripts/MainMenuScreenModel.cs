using Cysharp.Threading.Tasks;
using MVP.MVP_Root_Model.Scripts.Core;

namespace MVP.MVP_Root_Model.Scripts.Modules.MainMenuScreen.Scripts
{
    public class MainMenuScreenModel : IScreenModel
    {
        private readonly IRootController _rootController;
        private readonly MainMenuScreenPresenter _mainMenuScreenPresenter;

        public MainMenuScreenModel(IRootController rootController, MainMenuScreenPresenter mainMenuScreenPresenter)
        {
            _rootController = rootController;
            _mainMenuScreenPresenter = mainMenuScreenPresenter;
        }
        
        public async UniTask Run(object param)
        {
            _mainMenuScreenPresenter.Initialize(this);
            await _mainMenuScreenPresenter.ShowView();
            await _mainMenuScreenPresenter.WaitForTransitionButtonPress();
        }

        public void RunConverterModel() => _rootController.RunModel(ScreenModelMap.Converter);

        public void RunTicTacModel() => _rootController.RunModel(ScreenModelMap.TicTac);

        public async UniTask Stop() => await _mainMenuScreenPresenter.HideScreenView();
        public void Dispose() => _mainMenuScreenPresenter.RemoveEventListeners();
    }
}