using Modules.TestMenu.Scripts;
using Services.DataPersistenceSystem;
using Services.EnergyBar;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Startup
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private TestMenuUIView testMenuUIView;
        [SerializeField] private DataPersistenceManager dataPersistenceManager;

        protected override void Configure(IContainerBuilder builder)
        {            
            builder.RegisterInstance(mainCamera);
            
            builder.Register<EnergyBarService>(Lifetime.Singleton);

            builder.RegisterInstance<DataPersistenceManager>(dataPersistenceManager);
            
            builder.Register<TestMenu>(Lifetime.Transient);
        }
    }
}