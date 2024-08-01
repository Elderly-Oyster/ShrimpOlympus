using Core;
using Cysharp.Threading.Tasks;
using VContainer;

namespace Modules.Base.MainMenuScreen.Scripts
{
    public class MainMenuScreenPresenter : IPresenter
    {
        [Inject] private readonly MainMenuScreenView _mainMenuScreenView;
        private MainMenuScreenModel _mainMenuScreenModel; 
        

        public void Initialize(MainMenuScreenModel mainMenuScreenModel)
        {
            _mainMenuScreenModel = mainMenuScreenModel;
            _mainMenuScreenView.gameObject.SetActive(false);

            _mainMenuScreenView.SetupEventListeners
            (
                OnConverterButtonClicked,
                OnTicTacButtonClicked,
                OnFirstPopupButtonClicked,
                OnSecondPopupButtonClicked
            );
        }

        private void OnConverterButtonClicked() => _mainMenuScreenModel.RunConverterModel();
        private void OnTicTacButtonClicked() => _mainMenuScreenModel.RunTicTacModel();
        private void OnFirstPopupButtonClicked() => _mainMenuScreenModel.OpenFirstPopup();
        private void OnSecondPopupButtonClicked() => _mainMenuScreenModel.OpenSecondPopup();
        public async UniTask ShowView() => await _mainMenuScreenView.Show();
        public void RemoveEventListeners() => _mainMenuScreenView.RemoveEventListeners();
        public async UniTask HideScreenView() => await _mainMenuScreenView.Hide();
    }
}