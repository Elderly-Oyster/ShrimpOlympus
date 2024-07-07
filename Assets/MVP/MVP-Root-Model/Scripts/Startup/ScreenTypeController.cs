using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MVP.MVP_Root_Model.Scripts.Core;
using UnityEngine;
using VContainer;

namespace MVP.MVP_Root_Model.Scripts.Startup
{
    public class ScreenTypeController : MonoBehaviour, IRootController
    {
        [Inject] private ScreenTypeMapper _screenTypeMapper;
        private readonly SemaphoreSlim _semaphoreSlim = new (1, 1);
        private IScreenModel _currentModel;

        private void Start()
        {
            Debug.Log("ScreenTypeController Start");

            if (_screenTypeMapper == null)
            {
                Debug.LogError("ScreenTypeMapper is null");
            }
            else
            {
                Debug.Log("ScreenTypeMapper is not null");
            }

            RunModel(ScreenModelMap.StartGame).Forget();
        }

        public async UniTaskVoid RunModel(ScreenModelMap screenModelMap, object param = null)
        {
            Debug.Log($"Running model for {screenModelMap}");
            await _semaphoreSlim.WaitAsync();

            try
            {
                _currentModel = _screenTypeMapper.Resolve(screenModelMap);

                if (_currentModel == null)
                {
                    Debug.LogError($"Model for {screenModelMap} is null");
                    return;
                }

                await _currentModel.Run(param);
                await _currentModel.Stop();
                _currentModel.Dispose();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}