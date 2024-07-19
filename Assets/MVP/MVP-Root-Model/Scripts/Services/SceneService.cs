using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MVP.MVP_Root_Model.Scripts.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MVP.MVP_Root_Model.Scripts.Services
{
    public class SceneService
    {
        private CancellationTokenSource _cts;
        private List<string> _loadedScenes = new();

        public async UniTask LoadScenesForModule(ScreenModelMap screenModelMap)
        {
            List<string> scenes = new List<string> { screenModelMap.ToString() };
            IEnumerable<string> additionalScenes = GetAdditionalScenes(screenModelMap);
            if (additionalScenes != null)
                scenes.AddRange(additionalScenes);

            Debug.Log("Loading scenes: " + string.Join(", ", scenes));
            await LoadScenesAsync(scenes);
            await UnloadUnusedScenesAsync(scenes);
        }

        private IEnumerable<string> GetAdditionalScenes(ScreenModelMap screenModelMap)
        {
            return screenModelMap switch
            {
                ScreenModelMap.StartGame => null,
                ScreenModelMap.Converter => new List<string> { "PromotionGUI", "DynamicBackground"},
                ScreenModelMap.MainMenu => null,
                ScreenModelMap.TicTac => new List<string> { "PromotionGUI" },
                _ => null
            };
        }

        private bool IsCurrentScene(string sceneName) =>
            SceneManager.GetActiveScene().name == sceneName;

        private async UniTask LoadScenesAsync(List<string> scenes)
        {
            List<UniTask> loadTasks = new List<UniTask>();

            foreach (var scene in scenes)
            {
                if (!IsCurrentScene(scene))
                    loadTasks.Add(LoadSceneAsync(scene, true));
                else
                {
                    if (!_loadedScenes.Contains(scene))
                        _loadedScenes.Add(scene);
                }
            }

            await UniTask.WhenAll(loadTasks);
        }

        private async UniTask LoadSceneAsync(string sceneName, bool additive)
        {
            await LoadSceneAsyncInternal(() =>
                SceneManager.LoadSceneAsync(sceneName,
                    additive ? LoadSceneMode.Additive : LoadSceneMode.Single));

            if (!_loadedScenes.Contains(sceneName))
                _loadedScenes.Add(sceneName);
        }

        private async UniTask LoadSceneAsyncInternal(Func<AsyncOperation> loadSceneFunc)
        {
            try
            {
                var asyncOperation = loadSceneFunc();
                asyncOperation.allowSceneActivation = false;

                while (asyncOperation.progress < 0.9f)
                    await UniTask.Yield();

                asyncOperation.allowSceneActivation = true;
                await asyncOperation;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading scene: {ex.Message}");
            }
        }

        private async UniTask UnloadUnusedScenesAsync(List<string> scenesToLoad)
        {
            if (scenesToLoad == null || scenesToLoad.Count == 0)
            {
                Debug.LogWarning("No scenes to load.");
                return;
            }

            var scenesToUnload = _loadedScenes.Except(scenesToLoad).ToList();
            List<UniTask> unloadTasks = new List<UniTask>();

            foreach (var scene in scenesToUnload)
            {
                var sceneToUnload = SceneManager.GetSceneByName(scene);
                if (sceneToUnload.IsValid() && sceneToUnload.isLoaded)
                    unloadTasks.Add(SceneManager.UnloadSceneAsync(scene).ToUniTask());
                else
                    Debug.LogWarning($"Scene {scene} is not valid or not loaded.");
            }

            await UniTask.WhenAll(unloadTasks);

            _loadedScenes = _loadedScenes.Except(scenesToUnload).ToList();
        }
    }
}
