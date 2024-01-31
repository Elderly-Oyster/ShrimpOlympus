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

        private void Start() => RunPresenter(ScreenPresenterMap.StartGame).Forget();

        public async UniTaskVoid RunPresenter(ScreenPresenterMap screenPresenterMap, object param = null)
        {
            await _semaphoreSlim.WaitAsync();
            
            try
            {
                _currentPresenter = _screenTypeMapper.Resolve(screenPresenterMap);
                await _currentPresenter.Run(param);
                await _currentPresenter.Stop();
                _currentPresenter.Dispose();
            }
            finally { _semaphoreSlim.Release(); }
        }
    }
}