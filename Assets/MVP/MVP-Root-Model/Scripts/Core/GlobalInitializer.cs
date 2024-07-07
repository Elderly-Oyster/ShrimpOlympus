using MVP.MVP_Root_Model.Scripts.Installers;
using UnityEngine;

namespace MVP.MVP_Root_Model.Scripts.Core
{
    public static class GlobalInitializer
    {
        private static bool _isInitialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (_isInitialized) 
                return;
            var projectLifetimeScopePrefab = Resources.Load<ProjectLifetimeScope>("ProjectLifetimeScope");
            if (projectLifetimeScopePrefab != null)
            {
                var projectLifetimeScopeInstance = Object.Instantiate(projectLifetimeScopePrefab);
                Object.DontDestroyOnLoad(projectLifetimeScopeInstance.gameObject);
                _isInitialized = true;
            }
            else
                Debug.LogError("ProjectLifetimeScope prefab not found in Resources folder.");
        }
    }
}