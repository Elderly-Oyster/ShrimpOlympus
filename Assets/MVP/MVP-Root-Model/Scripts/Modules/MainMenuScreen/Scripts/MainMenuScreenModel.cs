using Cysharp.Threading.Tasks;
using MVP.MVP_Root_Model.Scripts.Core;
using MVP.MVP_Root_Model.Scripts.Core.Popup;
using MVP.MVP_Root_Model.Scripts.Core.Popup.Scripts;
using MVP.MVP_Root_Model.Scripts.Services;
using UnityEngine;
using VContainer;

namespace MVP.MVP_Root_Model.Scripts.Modules.MainMenuScreen.Scripts
{
    public class MainMenuScreenModel : IScreenModel
    {
        private readonly PopupHub _popupHub;
        private readonly IScreenController _screenController;
        private readonly MainMenuScreenPresenter _mainMenuScreenPresenter;

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

        public async UniTask Stop() => await _mainMenuScreenPresenter.HideScreenView();
        public void Dispose() => _mainMenuScreenPresenter.RemoveEventListeners();
    }
}