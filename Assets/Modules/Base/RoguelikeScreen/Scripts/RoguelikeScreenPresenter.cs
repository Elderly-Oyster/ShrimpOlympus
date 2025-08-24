using System.Threading;
using System.Threading.Tasks;
using CodeBase.Core.Infrastructure;
using CodeBase.Core.Infrastructure.Modules;
using Cysharp.Threading.Tasks;
using MediatR;
using Modules.Base.RoguelikeScreen.Scripts.Character;
using Modules.Base.RoguelikeScreen.Scripts.MediatorHandle;
using R3;
using Unit = R3.Unit;

namespace Modules.Base.RoguelikeScreen.Scripts
{
    public class RoguelikeStatePresenter : IModuleController, INotificationHandler<PlayerDeathNotification>,
        INotificationHandler<EnemyDeathNotification>
    {
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly RoguelikeScreenModel _screenModel;
        private readonly RoguelikeScreenView _screenView;
        private readonly UniTaskCompletionSource _screenCompletionSource;
        
        private readonly ReactiveCommand<Unit> _mainMenuCommand = new();
        private readonly ReactiveCommand<Unit> _restartCommand = new();
        private readonly CompositeDisposable _disposables = new();

        public RoguelikeStatePresenter(IScreenStateMachine screenStateMachine, 
            RoguelikeScreenModel screenModel, RoguelikeScreenView screenView, CharacterModel characterModel)
        {
            _screenStateMachine = screenStateMachine;
            _screenModel = screenModel;
            _screenView = screenView;
            _screenCompletionSource = new UniTaskCompletionSource();
            
            characterModel.Health.Subscribe(_screenView.SetHealth).AddTo(_disposables);
            _screenModel.Score.Subscribe(OnScoreChanged).AddTo(_disposables);
            
            _screenModel.OnGameplayEnterEvent += OnGameplayEnter;
            _screenModel.OnGameOverEnterEvent += OnGameOverEnter;
        }

        public async UniTask Enter(object param)
        {
            _screenView.HideInstantly();
            SubscribeToUIUpdates();
            _screenView.SetupEventListeners(_mainMenuCommand, _restartCommand);
            
            await _screenView.Show();
        }

        public async UniTask Execute() => await _screenCompletionSource.Task;

        public async UniTask Exit() => await _screenView.Hide();

        public void Dispose()
        {
            _screenView.Dispose();
            _screenModel.Dispose();
            _disposables.Dispose();
            
            _screenModel.OnGameplayEnterEvent -= OnGameplayEnter;
            _screenModel.OnGameOverEnterEvent -= OnGameOverEnter;
        }

        private void SubscribeToUIUpdates()
        {
            _mainMenuCommand.Subscribe(_ => OnMainMenuButtonClicked());
            _restartCommand.Subscribe(_ => OnRestartButtonClicked());
        }
            

        private void OnMainMenuButtonClicked() => 
            RunNewScreen(ModulesMap.MainMenu);

        private void OnRestartButtonClicked() =>
            _screenModel.TriggerStateChange(ScreenChangeTrigger.ToGameplay);

        private void RunNewScreen(ModulesMap screen)
        {
            _screenCompletionSource.TrySetResult();
            _screenStateMachine.RunModule(screen);
        }

        public Task Handle(PlayerDeathNotification notification, CancellationToken cancellationToken)
        {
            _screenModel.TriggerStateChange(ScreenChangeTrigger.ToGameOver);
            return MediatR.Unit.Task;
        }

        public Task Handle(EnemyDeathNotification notification, CancellationToken cancellationToken)
        {
            _screenModel.Score.Value += notification.PointsCount;
            return MediatR.Unit.Task;
        }
        
        public void OnScoreChanged(int newScore) => _screenView.SetScore(newScore);
        
        private void OnGameplayEnter()
        {
            _screenView.SetStatsEnabled(true);
            _screenView.SetOnDeathEnabled(false);
        }

        private void OnGameOverEnter()
        {
            _screenView.SetStatsEnabled(false);
            _screenView.SetOnDeathEnabled(true);
        }
    }
}
