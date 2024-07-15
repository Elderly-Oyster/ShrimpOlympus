using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MVP.MVP_Root_Model.Scripts.Services
{
    public class SceneService
    {
        private CancellationTokenSource _cts;

        public SceneService() { }

        private bool IsCurrentScene(int sceneIndex) =>
            SceneManager.GetActiveScene().buildIndex == sceneIndex;

        private bool IsCurrentScene(string sceneName) =>
            SceneManager.GetActiveScene().name == sceneName;

        public async UniTask OnLoadSceneAsync(string sceneName, bool additive = false)
        {
            if (IsCurrentScene(sceneName))
                return;
            await LoadSceneAsyncInternal(() => SceneManager.LoadSceneAsync(sceneName, additive ? LoadSceneMode.Additive : LoadSceneMode.Single));
        }

        public async UniTask OnLoadSceneAsync(int sceneIndex, bool additive = false)
        {
            if (IsCurrentScene(sceneIndex))
                return;
            await LoadSceneAsyncInternal(() => SceneManager.LoadSceneAsync(sceneIndex, additive ? LoadSceneMode.Additive : LoadSceneMode.Single));
        }

        private async UniTask LoadSceneAsyncInternal(Func<AsyncOperation> loadSceneFunc)
        {
            try
            {
                var asyncOperation = loadSceneFunc();
                asyncOperation.allowSceneActivation = false;

                while (asyncOperation.progress < 0.9f)
                {
                    await UniTask.Yield();
                }

                asyncOperation.allowSceneActivation = true;
                await asyncOperation;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading scene: {ex.Message}");
            }
        }
    }
}