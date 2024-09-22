using System;
using Cysharp.Threading.Tasks;
using VContainer;

namespace Modules.Additional.LoadingSplashScreen.Scripts
{
    public class SplashScreenService
    {
        [Inject] private LoadingSplashScreenPresenter _splashScreenPresenter;

        public async UniTask ShowSplashScreen() => 
            await _splashScreenPresenter.Enter();

        public async UniTask RunSplashScreen(Action scenesLoadingAction, Action servicesLoadingAction) => 
            await _splashScreenPresenter.DisplayLoading(scenesLoadingAction, servicesLoadingAction);

        public async UniTask HideSplashScreen()
        {
            await _splashScreenPresenter.Hide();
        }
    }
}