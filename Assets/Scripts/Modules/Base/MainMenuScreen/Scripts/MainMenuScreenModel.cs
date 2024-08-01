using Core;
using Core.Popup.Scripts;
using Cysharp.Threading.Tasks;

namespace Modules.Base.MainMenuScreen.Scripts
{
    public class MainMenuScreenModel : IScreenModel
    {
        private readonly IScreenController _screenController;
        private readonly MainMenuScreenPresenter _mainMenuScreenPresenter;
        private readonly PopupHub _popupHub;

        private readonly UniTaskCompletionSource<bool> _completionSource;

        public MainMenuScreenModel(IScreenController screenController,
            MainMenuScreenPresenter mainMenuScreenPresenter, PopupHub popupHub)
        {
            _completionSource = new UniTaskCompletionSource<bool>();
            _mainMenuScreenPresenter = mainMenuScreenPresenter;
            _screenController = screenController;
            _popupHub = popupHub;
        }
        
        public async UniTask Run(object param)
        {
            _mainMenuScreenPresenter.Initialize(this);
            await _mainMenuScreenPresenter.ShowView();
            await _completionSource.Task;
        }

        public void RunConverterModel()
        {
            _completionSource.TrySetResult(true);
            _screenController.RunModel(ScreenModelMap.Converter);
        }

        public void RunTicTacModel()
        {
            _completionSource.TrySetResult(true);
            _screenController.RunModel(ScreenModelMap.TicTac);
        }

        public void OpenFirstPopup() => _popupHub.OpenFirstPopup();
        public void OpenSecondPopup() => _popupHub.OpenSecondPopup();

        public async UniTask Stop() => await _mainMenuScreenPresenter.HideScreenView();
        public void Dispose() => _mainMenuScreenPresenter.RemoveEventListeners();
    }
}