using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Services
{
    public enum AdditiveScenesMap
    {
        PopupsManager,
        DynamicBackground
    }
    
    public class SceneService
    {
        private CancellationTokenSource _cts;
        private List<string> _loadedModuleScenes = new();
        private List<string> _staticModuleScenes = new();

        public SceneService()
        {
            AddStaticAdditiveScene(AdditiveScenesMap.PopupsManager);
            LoadStaticScenes().Forget();
        }
        
        public async UniTask LoadStaticScenes() //TODO Invoke from ScreenController?
        {
            await LoadScenesAsync(_staticModuleScenes);
        }
        
        public void AddStaticAdditiveScene(AdditiveScenesMap sceneName)
        {
            _staticModuleScenes.Add(sceneName.ToString());
        }
        
        public async UniTask LoadScenesForModule(ScreenPresenterMap screenPresenterMap)
        {
            List<string> scenes = new List<string> { screenPresenterMap.ToString() };
            IEnumerable<AdditiveScenesMap> additionalScenes = GetAdditionalScenes(screenPresenterMap);
            if (additionalScenes != null)
            {
                var sceneNames = additionalScenes.Select(scene => scene.ToString());
                scenes.AddRange(sceneNames);
            }

            Debug.Log("Loading scenes: " + string.Join(", ", scenes));
            await LoadScenesAsync(scenes);
            await UnloadUnusedScenesAsync(scenes);
        }
        
        private IEnumerable<AdditiveScenesMap> GetAdditionalScenes(ScreenPresenterMap screenPresenterMap)
        {
            return screenPresenterMap switch
            {
                ScreenPresenterMap.StartGame => new List<AdditiveScenesMap>(),
                ScreenPresenterMap.Converter => new List<AdditiveScenesMap> {AdditiveScenesMap.DynamicBackground},
                ScreenPresenterMap.MainMenu => new List<AdditiveScenesMap>(),
                ScreenPresenterMap.TicTac => new List<AdditiveScenesMap>(),
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
                    if (!_loadedModuleScenes.Contains(scene))
                        _loadedModuleScenes.Add(scene);
                }
            }

            await UniTask.WhenAll(loadTasks);
        }

        private async UniTask LoadSceneAsync(string sceneName, bool additive)
        {
            await LoadSceneAsyncInternal(() =>
                SceneManager.LoadSceneAsync(sceneName,
                    additive ? LoadSceneMode.Additive : LoadSceneMode.Single));

            if (!_loadedModuleScenes.Contains(sceneName))
                _loadedModuleScenes.Add(sceneName);
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

            var scenesToUnload = _loadedModuleScenes
                .Except(scenesToLoad)
                .Except(_staticModuleScenes) // Исключение постоянных сцен
                .ToList();

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

            _loadedModuleScenes = _loadedModuleScenes.Except(scenesToUnload).ToList();
        }
    }
}
