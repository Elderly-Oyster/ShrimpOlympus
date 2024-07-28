using Cysharp.Threading.Tasks;
using Scripts.Core;
using Scripts.Core.Popup.Scripts;

namespace Scripts.Modules.MainMenuScreen.Scripts
{
    public class MainMenuScreenModel : IScreenModel
    {
        private readonly IScreenController _screenController;
        private readonly MainMenuScreenPresenter _mainMenuScreenPresenter;
        private readonly PopupHub _popupHub;

        public MainMenuScreenModel(IScreenController screenController,
            MainMenuScreenPresenter mainMenuScreenPresenter, PopupHub popupHub)
        {
            _mainMenuScreenPresenter = mainMenuScreenPresenter;
            _screenController = screenController;
            _popupHub = popupHub;
        }
        
        public async UniTask Run(object param)
        {
            _mainMenuScreenPresenter.Initialize(this);
            await _mainMenuScreenPresenter.ShowView();
            await _mainMenuScreenPresenter.WaitForTransitionButtonPress();
        }

        public void RunConverterModel() => _screenController.RunModel(ScreenModelMap.Converter);

        public void RunTicTacModel() => _screenController.RunModel(ScreenModelMap.TicTac);

        public void OpenFirstPopup() => _popupHub.OpenFirstPopup();
        public void OpenSecondPopup() => _popupHub.OpenSecondPopup();

        public async UniTask Stop() => await _mainMenuScreenPresenter.HideScreenView();
        public void Dispose() => _mainMenuScreenPresenter.RemoveEventListeners();
    }
}