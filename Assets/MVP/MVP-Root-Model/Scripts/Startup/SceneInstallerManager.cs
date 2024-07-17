using System.Collections.Generic;
using MVP.MVP_Root_Model.Scripts.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace MVP.MVP_Root_Model.Scripts.Startup
{
    public class SceneInstallerManager
    {
        private static List<ISceneInstaller> FindAllSceneInstallers()
        {
            List<ISceneInstaller> sceneInstallers = new List<ISceneInstaller>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded)
                {
                    GameObject[] rootObjects = scene.GetRootGameObjects();
                    foreach (GameObject rootObject in rootObjects)
                    {
                        ISceneInstaller[] installersInRoot = rootObject.
                            GetComponentsInChildren<ISceneInstaller>(true);
                        sceneInstallers.AddRange(installersInRoot);
                    }
                }
            }

            return sceneInstallers;
        }

        public LifetimeScope CreateSceneLifetimeScope(LifetimeScope parentScope)
        {
            var newInstallers = FindAllSceneInstallers();
            return parentScope.CreateChild(builder =>
            {
                foreach (var installer in newInstallers)
                    installer.RegisterSceneDependencies(builder);
            });
        }
    }
}