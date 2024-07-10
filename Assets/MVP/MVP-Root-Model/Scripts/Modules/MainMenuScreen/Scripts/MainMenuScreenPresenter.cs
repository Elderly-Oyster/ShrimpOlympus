using Cysharp.Threading.Tasks;
using MVP.MVP_Root_Model.Scripts.Core;
using VContainer;

namespace MVP.MVP_Root_Model.Scripts.Modules.MainMenuScreen.Scripts
{
    public class MainMenuScreenPresenter : IPresenter
    {
        [Inject] private readonly MainMenuScreenView _mainMenuScreenView;
        private MainMenuScreenModel _mainMenuScreenModel; //Без инжекта, т.к. появлялась Circle Dependency Exception

        public void Initialize(MainMenuScreenModel mainMenuScreenModel)
        {
            _mainMenuScreenModel = mainMenuScreenModel;
            _mainMenuScreenView.gameObject.SetActive(false);

            _mainMenuScreenView.SetupEventListeners
            (
                OpenConverter,
                OpenFeature         
            );
        }

        private void OpenConverter()
        {
            _mainMenuScreenModel.OpenConverterState();
        }

        private void OpenFeature()
        {
            _mainMenuScreenModel.OpenFeatureState();
        }

        public async UniTask ShowView() => await _mainMenuScreenView.Show();
        public void RemoveEventListeners() => _mainMenuScreenView.RemoveEventListeners();
        public async UniTask HideScreenView() => await _mainMenuScreenView.Hide();
    }
}