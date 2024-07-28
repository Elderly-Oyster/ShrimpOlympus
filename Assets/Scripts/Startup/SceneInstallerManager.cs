using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Startup
{
    public class SceneInstallerManager
    {
        private List<ISceneInstaller> _currentScenesInstallers;
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

        public LifetimeScope CombineScenes(LifetimeScope parentScope)
        {
            _currentScenesInstallers = FindAllSceneInstallers();

            foreach (var installer in _currentScenesInstallers) 
                installer.RemoveObjectsToDelete();

            return parentScope.CreateChild(builder =>
            {
                foreach (var installer in _currentScenesInstallers)
                    installer.RegisterSceneDependencies(builder);
            });
        }
    }
}