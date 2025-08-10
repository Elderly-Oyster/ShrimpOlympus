using System;
using CodeBase.Core.Infrastructure;
using CodeBase.Core.Infrastructure.Modules;
using Cysharp.Threading.Tasks;
using MediatR;
using R3;
using VContainer;

namespace Modules.Base.BonusGameModule.Scripts
{
    /// <summary>
        /// Main controller for BonusGame module that manages the module lifecycle
        /// and coordinates between Presenter, Model and View
        /// 
        /// IMPORTANT: This is a bonusGame file for ModuleCreator system.
        /// When creating a new module, this file will be copied and modified.
        /// 
        /// Key points for customization:
        /// 1. Change class name from BonusGameModuleController to YourModuleNameModuleController
        /// 2. Update namespace Modules.Base.BonusGameModule.Scripts match your module location
        /// 3. Customize module lifecycle management if needed
        /// 4. Add specific initialization logic for your module
        /// 5. Implement custom exit conditions if required
    /// </summary>
    public class BonusGameModuleController : IModuleController
    {
        [Inject] private IMediator _mediator;
        private readonly UniTaskCompletionSource _moduleCompletionSource;
        private readonly BonusGameModuleModel _bonusGameModuleModel;
        private readonly BonusGamePresenter _bonusGamePresenter;
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly ReactiveCommand<ModulesMap> _openNewModuleCommand = new();
        
        private readonly CompositeDisposable _disposables = new();
        
        public BonusGameModuleController(IScreenStateMachine screenStateMachine, BonusGameModuleModel bonusGameModuleModel, 
            BonusGamePresenter bonusGamePresenter)
        {
            _bonusGameModuleModel = bonusGameModuleModel ?? throw new ArgumentNullException(nameof(bonusGameModuleModel));
            _bonusGamePresenter = bonusGamePresenter ?? throw new ArgumentNullException(nameof(bonusGamePresenter));
            _screenStateMachine = screenStateMachine ?? throw new ArgumentNullException(nameof(screenStateMachine));
            
            _moduleCompletionSource = new UniTaskCompletionSource();
        }

        public async UniTask Enter(object param)
        {
            SubscribeToModuleUpdates();

            _bonusGamePresenter.HideInstantly();
            
            await _bonusGamePresenter.Enter(_openNewModuleCommand);
        }

        public async UniTask Execute() => await _moduleCompletionSource.Task;

        public async UniTask Exit()
        {
            await _bonusGamePresenter.Exit();
        }

        public void Dispose()
        {
            _disposables.Dispose();
            
            _bonusGamePresenter.Dispose();
            
            _bonusGameModuleModel.Dispose();
        }

        private void SubscribeToModuleUpdates()
        {
            // Prevent rapid module switching
            _openNewModuleCommand
                .ThrottleFirst(TimeSpan.FromMilliseconds(_bonusGameModuleModel.ModuleTransitionThrottleDelay))
                .Subscribe(RunNewModule)
                .AddTo(_disposables);
        }

        private void RunNewModule(ModulesMap screen)
        {
            _moduleCompletionSource.TrySetResult();
            _screenStateMachine.RunModule(screen);
        }
    }
}
