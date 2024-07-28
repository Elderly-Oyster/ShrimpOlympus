using Cysharp.Threading.Tasks;
using Scripts.Core;
using VContainer;

namespace Scripts.Modules.NewScreen.Scripts
{
    public class NewScreenPresenter : IPresenter
    {
        [Inject] private readonly NewScreenView _ticTacScreenView;
        private NewScreenModel _ticTacScreenModel; 
        private readonly UniTaskCompletionSource<bool> _completionSource = new();

        public void Initialize(NewScreenModel ticTacScreenModel)
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