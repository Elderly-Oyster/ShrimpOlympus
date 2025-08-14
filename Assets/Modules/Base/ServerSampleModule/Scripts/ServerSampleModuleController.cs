using System;
using CodeBase.Core.Infrastructure;
using CodeBase.Core.Infrastructure.Modules;
using Cysharp.Threading.Tasks;
using MediatR;
using R3;
using VContainer;

namespace Modules.Base.ServerSampleModule.Scripts
{
    /// <summary>
        /// Main controller for ServerSample module that manages the module lifecycle
        /// and coordinates between Presenter, Model and View
        /// 
        /// IMPORTANT: This is a serverSample file for ModuleCreator system.
        /// When creating a new module, this file will be copied and modified.
        /// 
        /// Key points for customization:
        /// 1. Change class name from ServerSampleModuleController to YourModuleNameModuleController
        /// 2. Update namespace Modules.Base.ServerSampleModule.Scripts match your module location
        /// 3. Customize module lifecycle management if needed
        /// 4. Add specific initialization logic for your module
        /// 5. Implement custom exit conditions if required
    /// </summary>
    public class ServerSampleModuleController : IModuleController
    {
        [Inject] private IMediator _mediator;
        private readonly UniTaskCompletionSource _moduleCompletionSource;
        private readonly ServerSampleModuleModel _serverSampleModuleModel;
        private readonly ServerSamplePresenter _serverSamplePresenter;
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly ReactiveCommand<ModulesMap> _openNewModuleCommand = new();
        
        private readonly CompositeDisposable _disposables = new();
        
        public ServerSampleModuleController(IScreenStateMachine screenStateMachine, ServerSampleModuleModel serverSampleModuleModel, 
            ServerSamplePresenter serverSamplePresenter)
        {
            _serverSampleModuleModel = serverSampleModuleModel ?? throw new ArgumentNullException(nameof(serverSampleModuleModel));
            _serverSamplePresenter = serverSamplePresenter ?? throw new ArgumentNullException(nameof(serverSamplePresenter));
            _screenStateMachine = screenStateMachine ?? throw new ArgumentNullException(nameof(screenStateMachine));
            
            _moduleCompletionSource = new UniTaskCompletionSource();
        }

        public async UniTask Enter(object param)
        {
            SubscribeToModuleUpdates();

            _serverSamplePresenter.HideInstantly();
            
            await _serverSamplePresenter.Enter(_openNewModuleCommand);
        }

        public async UniTask Execute() => await _moduleCompletionSource.Task;

        public async UniTask Exit()
        {
            await _serverSamplePresenter.Exit();
        }

        public void Dispose()
        {
            _disposables.Dispose();
            
            _serverSamplePresenter.Dispose();
            
            _serverSampleModuleModel.Dispose();
        }

        private void SubscribeToModuleUpdates()
        {
            // Prevent rapid module switching
            _openNewModuleCommand
                .ThrottleFirst(TimeSpan.FromMilliseconds(_serverSampleModuleModel.ModuleTransitionThrottleDelay))
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
