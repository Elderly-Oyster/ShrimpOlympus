using Cysharp.Threading.Tasks;
using VContainer;
using System.Collections.Generic;
using Core;

namespace Modules.Test.PopupsTester.Scripts
{
    public class PopupsTesterScenePresenter : IPresenter
    {
        [Inject] private readonly PopupsTesterSceneView _popupsTesterSceneView;
        private PopupsTesterSceneModel _popupsTesterSceneModel;

        public void Initialize(PopupsTesterSceneModel popupsTesterSceneModel)
        {
            _popupsTesterSceneModel = popupsTesterSceneModel;
            _popupsTesterSceneView.SetupEventListeners(_popupsTesterSceneModel.GetButtons());
            _popupsTesterSceneView.gameObject.SetActive(false);
        }
        
        public async UniTask ShowView() => await _popupsTesterSceneView.Show();
        
        public void RemoveEventListeners() => _popupsTesterSceneView.RemoveEventListeners();
        
        public async UniTask HideScreenView() => await _popupsTesterSceneView.Hide();
    }
}