using Core;
using Core.MVVM;
using Core.Popup.Scripts;
using Cysharp.Threading.Tasks;

namespace Modules.Base.MainMenuScreen.Scripts
{
    public class MainMenuScreenModel : IScreenModel
    {
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly MainMenuScreenPresenter _mainMenuScreenPresenter;
        private readonly PopupHub _popupHub;

        private readonly UniTaskCompletionSource<bool> _completionSource;

        public MainMenuScreenModel(IScreenStateMachine screenStateMachine,
            MainMenuScreenPresenter mainMenuScreenPresenter, PopupHub popupHub)
        {
            _completionSource = new UniTaskCompletionSource<bool>();
            _mainMenuScreenPresenter = mainMenuScreenPresenter;
            _screenStateMachine = screenStateMachine;
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
            _screenStateMachine.RunViewModel(ScreenPresenterMap.Converter);
        }

        public void RunTicTacModel()
        {
            _completionSource.TrySetResult(true);
            _screenStateMachine.RunViewModel(ScreenPresenterMap.TicTac);
        }

        public void OpenFirstPopup() => _popupHub.OpenFirstPopup();
        public void OpenSecondPopup() => _popupHub.OpenSecondPopup();

        public async UniTask Stop() => await _mainMenuScreenPresenter.HideScreenView();
        public void Dispose() => _mainMenuScreenPresenter.RemoveEventListeners();
    }
}