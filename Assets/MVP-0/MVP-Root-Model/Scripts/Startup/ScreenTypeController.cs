using System.Threading;
using Cysharp.Threading.Tasks;
using MVP_0.MVP_Root_Model.Scripts.Core;
using UnityEngine;
using VContainer;

namespace MVP_0.MVP_Root_Model.Scripts.Startup
{
    public class ScreenTypeController : MonoBehaviour, IRootController
    {
        [Inject] private ScreenTypeMapper _screenTypeMapper;
        private readonly SemaphoreSlim _semaphoreSlim = new (1, 1);
        private IScreenModel _currentModel;

        private void Start() => RunModel(ScreenModelMap.StartGame).Forget();

        public async UniTaskVoid RunModel(ScreenModelMap screenModelMap, object param = null)
        {
            await _semaphoreSlim.WaitAsync();
            
            try
            {
                _currentModel = _screenTypeMapper.Resolve(screenModelMap);
                await _currentModel.Run(param);
                await _currentModel.Stop();
                _currentModel.Dispose();
            }
            finally { _semaphoreSlim.Release(); }
        }
    }
}