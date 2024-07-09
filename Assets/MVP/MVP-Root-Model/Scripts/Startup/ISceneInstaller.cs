using UnityEngine;
using VContainer.Unity;

namespace MVP.MVP_Root_Model.Scripts.Startup
{
    public interface ISceneInstaller
    {
        public LifetimeScope CreateSceneLifetimeScope(LifetimeScope currentScope);
    }
}