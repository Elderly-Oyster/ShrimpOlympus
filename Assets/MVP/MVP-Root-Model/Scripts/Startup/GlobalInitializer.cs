using UnityEngine;

namespace MVP.MVP_Root_Model.Scripts.Startup
{
    public static class GlobalInitializer
    {
        /*private static bool _isInitialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (!_isInitialized)
            {
                var projectLifetimeScopePrefab = Resources.Load<RootLifetimeScope>("ProjectLifetimeScope");
                if (projectLifetimeScopePrefab != null)
                {
                    var projectLifetimeScopeInstance = Object.Instantiate(projectLifetimeScopePrefab);
                    Object.DontDestroyOnLoad(projectLifetimeScopeInstance.gameObject);
                    _isInitialized = true;
                }
                else
                    Debug.LogError("ProjectLifetimeScope prefab not found in Resources folder.");
            }
        }*/
    }
}