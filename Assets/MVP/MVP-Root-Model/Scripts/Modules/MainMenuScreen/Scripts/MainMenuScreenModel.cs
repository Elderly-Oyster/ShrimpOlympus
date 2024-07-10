using System;
using Cysharp.Threading.Tasks;
using MVP.MVP_Root_Model.Scripts.Core;

namespace MVP.MVP_Root_Model.Scripts.Modules.MainMenuScreen.Scripts
{
    public class MainMenuScreenModel : IScreenModel
    {
        private readonly IRootController _rootController;
        private readonly MainMenuScreenPresenter _mainMenuScreenPresenter;
        private readonly UniTaskCompletionSource<Action> _completionSource;

        public MainMenuScreenModel(IRootController rootController, MainMenuScreenPresenter mainMenuScreenPresenter)
        {
            _completionSource = new UniTaskCompletionSource<Action>();
            _rootController = rootController;
            _mainMenuScreenPresenter = mainMenuScreenPresenter;
        }
        
        public async UniTask Run(object param)
        {
            _mainMenuScreenPresenter.Initialize(this);
            await _mainMenuScreenPresenter.ShowView();
            var result = await _completionSource.Task;
            result.Invoke();
        }

        public void OpenConverterState()
        {
            _rootController.RunModel(ScreenModelMap.Converter);
        }

        public void OpenFeatureState()
        {
            //_rootController.RunModel(ScreenModelMap.Feature);
        }

        public async UniTask Stop() => await _mainMenuScreenPresenter.HideScreenView();
        public void Dispose() => _mainMenuScreenPresenter.RemoveEventListeners();
    }
}