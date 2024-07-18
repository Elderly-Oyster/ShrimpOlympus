using VContainer;

namespace MVP.MVP_Root_Model.Scripts.Core
{
    public interface ISceneInstaller
    {
        public void RegisterSceneDependencies(IContainerBuilder builder);

        public void RemoveObjectsToDelete();
    }
}