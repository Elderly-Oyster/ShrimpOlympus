using Cysharp.Threading.Tasks;
using Startup;
using VContainer;
using VContainer.Unity;

namespace Modules.Additional.LoadingSplashScreen.Scripts
{
    public class SplashScreenService : IStartable
    {
        [Inject] private ScreenStateMachine _screenStateMachine;
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

        public void Start() => _screenStateMachine.ModuleChanged += OnModuleChanged;

        private void OnModuleChanged(IObjectResolver resolver) => EnsureAudioListenerExists(resolver);

    }
}