using Cysharp.Threading.Tasks;
using MVP.MVP_Root_Model.Scripts.Core;
using VContainer;

namespace MVP.MVP_Root_Model.Scripts.Modules.MainMenuScreen.Scripts
{
    public class MainMenuScreenPresenter : IPresenter
    {
        [Inject] private readonly MainMenuScreenView _mainMenuScreenView;
        private MainMenuScreenModel _mainMenuScreenModel; 
        
        private readonly UniTaskCompletionSource<bool> _playButtonPressed = new();

        public void Initialize(MainMenuScreenModel mainMenuScreenModel)
        {
            _mainMenuScreenModel = mainMenuScreenModel;
            _mainMenuScreenView.gameObject.SetActive(false);

            _mainMenuScreenView.SetupEventListeners
            (
                OnConverterButtonPressed,
                OnTicTacButtonPressed         
            );
        }

        private void OnConverterButtonPressed()
        {
            _mainMenuScreenModel.RunConverterModel();
            _playButtonPressed.TrySetResult(true);
        }

        private void OnTicTacButtonPressed()
        {
            _mainMenuScreenModel.RunTicTacModel();
            _playButtonPressed.TrySetResult(true);
        }
        
        public async UniTask WaitForPlayButtonPress() => await _playButtonPressed.Task;

        public async UniTask ShowView() => await _mainMenuScreenView.Show();
        public void RemoveEventListeners() => _mainMenuScreenView.RemoveEventListeners();
        public async UniTask HideScreenView() => await _mainMenuScreenView.Hide();
    }
}