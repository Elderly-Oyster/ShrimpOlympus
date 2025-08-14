using CodeBase.Core;
using CodeBase.Services.SceneInstallerService;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Base.ServerSampleModule.Scripts
{
    /// <summary>
    /// Installer for ServerSample module that registers all dependencies
    /// 
    /// IMPORTANT: This is a serverSample file for ModuleCreator system.
    /// When creating a new module, this file will be copied and modified.
    /// 
    /// Key points for customization:
    /// 1. Change class name from ServerSampleModuleInstaller to YourModuleNameInstaller
    /// 2. Update namespace Modules.Base.ServerSampleModule.Scripts match your module location
    /// 3. Register your specific dependencies
    /// 4. Update the View component reference
    /// 5. Add any additional services or systems your module needs
    /// </summary>
    public class ServerSampleModuleInstaller : BaseModuleSceneInstaller
    {
        [SerializeField] private ServerSampleView serverSampleView;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            base.RegisterSceneDependencies(builder);

            builder.AddMediatR(typeof(ServerSampleHandler).Assembly);
            
            // Register main module controller
            builder.Register<ServerSampleModuleController>(Lifetime.Singleton);
            
            // Register MVP components
            builder.Register<ServerSampleModuleModel>(Lifetime.Singleton);
            builder.Register<ServerSamplePresenter>(Lifetime.Singleton);
            builder.RegisterComponent(serverSampleView).As<ServerSampleView>();
        }
    }
}