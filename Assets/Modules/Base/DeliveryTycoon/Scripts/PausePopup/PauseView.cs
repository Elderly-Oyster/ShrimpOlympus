using CodeBase.Core.Infrastructure;
using CodeBase.Core.Modules;
using CodeBase.Core.Systems.Save;
using CodeBase.Services;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Modules.Base.DeliveryTycoon.Scripts.PausePopup
{
    public class PauseView : BaseView
    {
        private const float SlowdownDuration = 0.5f;
        private const float SpeedupDuration = 0.4f;
        private const float PausedTimeScale = 0f;
        private const float NormalTimeScale = 1f;
        
        [SerializeField] private Button resumeGameButton;
        [SerializeField] private Button mainMenuButton;
        
        private InputSystemService _inputSystemService;
        private SaveSystem _saveSystem;
        private Tween _timeScaleTween;

        [Inject]
        public void Construct(InputSystemService inputSystemService, SaveSystem saveSystem)
        {
            _inputSystemService = inputSystemService;
            _saveSystem = saveSystem;
        }

        public void SetupEventListeners(
            ReactiveCommand<ModulesMap> openNewModuleCommand,
            ReactiveCommand<Unit> exitPauseCommand)
        {
            var cancelPerformedObservable = 
                _inputSystemService.GetPerformedObservable(_inputSystemService.InputActions.UI.Cancel);
            
            cancelPerformedObservable
                .Where(_ => IsActive)
                .Subscribe(_ => exitPauseCommand.Execute(default))
                .AddTo(this);
            
            resumeGameButton.OnClickAsObservable()
                .Subscribe(_ => exitPauseCommand.Execute(default))
                .AddTo(this); 
            
            mainMenuButton.OnClickAsObservable()
                .Subscribe( _ => openNewModuleCommand.Execute(ModulesMap.MainMenu))
                .AddTo(this); 
        }
        
        public override async UniTask Show()
        {
            _saveSystem.SaveData().Forget();
            _timeScaleTween?.Kill();
            
            // _timeScaleTween = DOTween.To(
            //         () => Time.timeScale, 
            //         value => Time.timeScale = value,
            //         PausedTimeScale, 
            //         SlowdownDuration 
            //     ).SetEase(Ease.OutQuad)
            //     .SetUpdate(UpdateType.Normal, true);

            _inputSystemService.SwitchToUI();
            _inputSystemService.SetFirstSelectedObject(resumeGameButton);
            
            await base.Show();
            Debug.Log("Pause View state is " + IsActive);
        }
        
        public override async UniTask Hide()
        {
            Debug.Log("Pause View state is " + IsActive);
            if (!IsActive) return;
            
            // _timeScaleTween?.Kill();
            // _timeScaleTween = DOTween.To(
            //         () => Time.timeScale, 
            //         value => Time.timeScale = value, 
            //         NormalTimeScale, 
            //         SpeedupDuration 
            //     ).SetEase(Ease.InQuad)
            //     .SetUpdate(UpdateType.Normal, true);
            
            await base.Hide();
        }
        
        public override void Dispose()
        {
            _timeScaleTween?.Kill();
            Time.timeScale = NormalTimeScale;
            base.Dispose();
        }
    }
}