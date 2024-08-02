using Core;
using Cysharp.Threading.Tasks;
using VContainer;

namespace Modules.Base.NewScreen.Scripts
{
    public class NewScreenPresenter : IPresenter
    {
        [Inject] private readonly NewScreenView _newScreenView;
        private NewScreenModel _ticTacScreenModel; 

        public void Initialize(NewScreenModel ticTacScreenModel)
        {
            _ticTacScreenModel = ticTacScreenModel;
            _newScreenView.SetupEventListeners(OnMainMenuButtonClicked);
        }

        public async UniTask ShowView() => await _newScreenView.Show();
        
        private void OnMainMenuButtonClicked() => _ticTacScreenModel.RunMainMenuModel();

        public void RemoveEventListeners() => _newScreenView.RemoveEventListeners();
        public async UniTask HideScreenView() => await _newScreenView.Hide();
    }
}