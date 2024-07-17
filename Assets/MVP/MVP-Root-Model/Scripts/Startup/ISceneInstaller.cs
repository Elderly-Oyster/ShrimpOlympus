using VContainer;

namespace MVP.MVP_Root_Model.Scripts.Startup
{
    public interface ISceneInstaller
    {
        public void RegisterSceneDependencies(IContainerBuilder builder);
    }
}