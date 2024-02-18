using System.Threading;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Startup
{
    public class ScreenTypeController : MonoBehaviour, IRootController
    {
        [Inject] private ScreenTypeMapper _screenTypeMapper;
        private readonly SemaphoreSlim _semaphoreSlim = new (1, 1);
        private IScreenPresenter _currentPresenter;

        private void Start() => RunModel(ScreenModelMap.StartGame).Forget();

        public async UniTaskVoid RunModel(ScreenModelMap screenModelMap, object param = null)
        {
            await _semaphoreSlim.WaitAsync();
            
            try
            {
                _currentPresenter = _screenTypeMapper.Resolve(screenModelMap);
                await _currentPresenter.Run(param);
                await _currentPresenter.Stop();
                _currentPresenter.Dispose();
            }
            finally { _semaphoreSlim.Release(); }
        }
    }
}