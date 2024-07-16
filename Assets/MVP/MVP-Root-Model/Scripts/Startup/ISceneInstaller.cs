using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MVP.MVP_Root_Model.Scripts.Startup
{
    public interface ISceneInstaller
    {
        public void RegisterDependencies(IContainerBuilder builder);
    }
}