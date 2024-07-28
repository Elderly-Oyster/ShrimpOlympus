using VContainer;

namespace Core
{
    public interface ISceneInstaller
    {
        public void RegisterSceneDependencies(IContainerBuilder builder);

        public void RemoveObjectsToDelete();
    }
}