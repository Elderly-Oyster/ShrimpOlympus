using System;
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

        public async UniTask OnLoadSceneAsync(string sceneName)
        {
            if (IsCurrentScene(sceneName)) return;

            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            try
            {
                await LoadingSceneAsync(sceneName, _cts.Token);
            }
            catch (OperationCanceledException exp)
            {
                if (exp.CancellationToken == _cts.Token)
                {
                    Debug.LogWarning("Task cancelled");
                }
            }
            finally
            {
                _cts.Cancel();
                _cts = null;
            }
        }

        public async UniTask OnLoadSceneAsync(int sceneIndex)
        {
            if (IsCurrentScene(sceneIndex)) return;

            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            try
            {
                await LoadingSceneAsync(sceneIndex, _cts.Token);
            }
            catch (OperationCanceledException exp)
            {
                if (exp.CancellationToken == _cts.Token)
                {
                    Debug.LogWarning("Task cancelled");
                }
            }
            finally
            {
                _cts.Cancel();
                _cts = null;
            }
        }

        private async Task LoadingSceneAsync(string sceneName, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            asyncOperation.allowSceneActivation = false;

            while (asyncOperation.progress < 0.9f)
            {
                token.ThrowIfCancellationRequested();
                UpdateProgress(asyncOperation.progress);
                await UniTask.Yield();
            }

            await UniTask.Delay(1500, cancellationToken: token);
            asyncOperation.allowSceneActivation = true;
            await asyncOperation;
        }

        private async Task LoadingSceneAsync(int sceneIndex, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
            asyncOperation.allowSceneActivation = false;

            while (asyncOperation.progress < 0.9f)
            {
                token.ThrowIfCancellationRequested();
                UpdateProgress(asyncOperation.progress);
                await UniTask.Yield();
            }

            await UniTask.Delay(1500, cancellationToken: token);
            asyncOperation.allowSceneActivation = true;
            await asyncOperation;
        }

        private void UpdateProgress(float progress)
        {
            float percentage = progress * 100;
            int percentageInt = (int)Math.Round(percentage, 0);
            //Debug.Log($"Loading progress: {percentageInt}%");
        }
    }
}
