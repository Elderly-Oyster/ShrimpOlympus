using Cysharp.Threading.Tasks;
using VContainer;

namespace Modules.Additional.LoadingSplashScreen.Scripts
{
    public class SplashScreenService
    {
        [Inject] private LoadingSplashScreenPresenter _splashScreenPresenter;

        public async UniTask ShowSplashScreen() => 
            await _splashScreenPresenter.Show();

        public void UpdateScenesProgress(float progressData)
        {
            _splashScreenPresenter.UpdateSceneLoadingProgress(progressData);
        }
        
        public async UniTask ExecuteSplashScreen()
        {
            await _splashScreenPresenter.Execute();
        }

        public async UniTask HideSplashScreen()
        {
            await _splashScreenPresenter.Hide();
        }
    }
}