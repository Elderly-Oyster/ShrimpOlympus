using Cysharp.Threading.Tasks;
using MVP.MVP_Root_Model.Scripts.Core;
using VContainer;

namespace MVP.MVP_Root_Model.Scripts.Modules.MainMenuScreen.Scripts
{
    public class MainMenuScreenPresenter : IPresenter
    {
        [Inject] private readonly MainMenuScreenView _mainMenuScreenView;
        private MainMenuScreenModel _mainMenuScreenModel; 
        
        private readonly UniTaskCompletionSource<bool> _completionSource = new();

        public void Initialize(MainMenuScreenModel mainMenuScreenModel)
        {
            _mainMenuScreenModel = mainMenuScreenModel;
            _mainMenuScreenView.gameObject.SetActive(false);

            _mainMenuScreenView.SetupEventListeners
            (
                OnConverterButtonClicked,
                OnTicTacButtonClicked,
                OnFirstPopupButtonClicked
            );
        }

        private void OnConverterButtonClicked()
        {
            _mainMenuScreenModel.RunConverterModel();
            _completionSource.TrySetResult(true);
        }

        private void OnTicTacButtonClicked()
        {
            _mainMenuScreenModel.RunTicTacModel();
            _completionSource.TrySetResult(true);
        }

        private void OnFirstPopupButtonClicked() => _mainMenuScreenModel.OpenFirstPopup();

        public async UniTask WaitForTransitionButtonPress() => await _completionSource.Task;

        public async UniTask ShowView() => await _mainMenuScreenView.Show();
        public void RemoveEventListeners() => _mainMenuScreenView.RemoveEventListeners();
        public async UniTask HideScreenView() => await _mainMenuScreenView.Hide();
    }
}