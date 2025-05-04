using VContainer;

namespace CodeBase.Core.Modules.Installer
{
    public interface ISceneInstaller
    {
        public void RegisterSceneDependencies(IContainerBuilder builder);

        public void RemoveObjectsToDelete();

        public void InjectSceneViews(IObjectResolver resolver);
    }
}