using Core;
using Cysharp.Threading.Tasks;
using VContainer;

namespace Modules.Base.NewScreen.Scripts
{
    public class NewScreenPresenter : IPresenter
    {
        [Inject] private readonly NewScreenView _newScreenView;
        private NewScreenModel _ticTacScreenModel; 
        private readonly UniTaskCompletionSource<bool> _completionSource = new();

        public void Initialize(NewScreenModel ticTacScreenModel)
        {
            _ticTacScreenModel = ticTacScreenModel;
            _newScreenView.gameObject.SetActive(false);
            _newScreenView.SetupEventListeners(OnMainMenuButtonClicked);
        }

        public async UniTask ShowView() => await _newScreenView.Show();
        
        private void OnMainMenuButtonClicked()
        {
            _ticTacScreenModel.RunMainMenuModel();
            _completionSource.TrySetResult(true);
        }
        public async UniTask WaitForTransitionButtonPress() => await _completionSource.Task;

        public void RemoveEventListeners() => _newScreenView.RemoveEventListeners();
        public async UniTask HideScreenView() => await _newScreenView.Hide();
    }
}