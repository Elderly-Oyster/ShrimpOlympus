using Cysharp.Threading.Tasks;
using MVP.MVP_Root_Model.Scripts.Core;
using VContainer;

namespace MVP.MVP_Root_Model.Scripts.Modules.TicTacScreen.Scripts
{
    public class TicTacScreenPresenter : IPresenter
    {
        [Inject] private readonly TicTacScreenView _ticTacScreenView;
        private TicTacScreenModel _ticTacScreenModel; 
        private readonly UniTaskCompletionSource<bool> _completionSource = new();

        public void Initialize(TicTacScreenModel ticTacScreenModel)
        {
            _ticTacScreenModel = ticTacScreenModel;
            _ticTacScreenView.gameObject.SetActive(false);
            _ticTacScreenView.SetupEventListeners(OnMainMenuButtonClicked);
        }

        public async UniTask ShowView() => await _ticTacScreenView.Show();
        
        private void OnMainMenuButtonClicked()
        {
            _ticTacScreenModel.RunMainMenuModel();
            _completionSource.TrySetResult(true);
        }
        public async UniTask WaitForTransitionButtonPress() => await _completionSource.Task;

        public void RemoveEventListeners() => _ticTacScreenView.RemoveEventListeners();
        public async UniTask HideScreenView() => await _ticTacScreenView.Hide();
    }
}