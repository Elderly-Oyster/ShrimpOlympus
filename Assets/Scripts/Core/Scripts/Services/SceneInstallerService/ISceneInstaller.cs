using VContainer;

namespace Core.Scripts.Services.SceneInstallerService
{
    public interface ISceneInstaller
    {
        public void RegisterSceneDependencies(IContainerBuilder builder);

        public void RemoveObjectsToDelete();

        public void InjectSceneViews(IObjectResolver resolver);
    }
}