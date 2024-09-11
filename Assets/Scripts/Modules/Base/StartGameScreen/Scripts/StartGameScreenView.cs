using System;
using System.Threading;
using Core.MVP;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Modules.Base.StartGameScreen.Scripts
{
    public class StartGameScreenView : BaseScreenView
    {
        [Header("UI Interaction Components")]
        [SerializeField] private Button continueButton;

        [Header("Progress UI Components")]
        [SerializeField] private TMP_Text progressValueText;
        [SerializeField] private TMP_Text progressText;
        [SerializeField] private Image progressBar;
        [SerializeField] private CanvasGroup progressBarCanvasGroup;

        [Header("Dynamic UI Visuals")]
        [SerializeField] private CanvasGroup lightingCanvasGroup; // Это не реализовывал, мб позже
        [SerializeField] private Image stuffImage;
        [SerializeField] private Image overlay;

        [Header("Splash Screen Components")]
        [SerializeField] private TMP_Text splashTooltipsText;
        [SerializeField] private TMP_Text versionText;
        
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private Sequence _sequence;
        
        private const string TapToContinueText = "Tap to continue";
        private const float ProgressBarAnimDuration = 0.5f;
        private const float FlickerDuration = .2f;
        
        private void ResetProgressBar() => progressBar.fillAmount = 0;

        private void Start() => splashTooltipsText.transform.parent.gameObject.SetActive(true);

        public void SetupEventListeners(Action onStartButtonClicked) =>
            continueButton.OnClickAsObservable()
                .Subscribe(_ => onStartButtonClicked())
                .AddTo(_disposables);
        
        public void SetVersionText(string version) => versionText.text = version;

        public UniTask ReportProgress(float expProgress, string progressStatus)
        {
            progressText.text = progressStatus;
            
            return DOTween.To(() => progressBar.fillAmount, x =>
            {
                progressBar.fillAmount = x;
                progressValueText.text = $"{(int)(x * 100)}%";
                
                overlay.color = new Color(0, 0, 0, 1 - expProgress);
                lightingCanvasGroup.alpha = expProgress;
                stuffImage.color = new Color(1, 1, 1, Math.Max(.1f, expProgress));
            }, expProgress, 1f).ToUniTask(); 
        }

        public void ReportText(string text) => progressText.text = text; // Если можно резделить инициализацию сервиса
        
        public void SetTooltipText(string text) =>
            splashTooltipsText.text = text;
        
        public void ShowAnimations(CancellationToken cancellationToken)
        {
            progressText.text = TapToContinueText;

            _sequence = DOTween.Sequence();
            _sequence
                .Append(progressText.transform
                    .DOScale(1.2f, ProgressBarAnimDuration))
                .SetLoops(-1, LoopType.Yoyo); // Переместили SetLoops к последовательности

            
            progressBarCanvasGroup.DOFade(0, ProgressBarAnimDuration);
            
            StartFlickering(cancellationToken).Forget();
        }

        private async UniTaskVoid StartFlickering(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                const Ease easy = Ease.Flash;

                var opacity = Random.Range(0f, .3f);
                
                await UniTask.WhenAll(
                    lightingCanvasGroup
                        .DOFade(opacity, FlickerDuration)
                        .SetEase(easy)
                        .ToUniTask(cancellationToken: cancellationToken),
                    overlay
                        .DOFade(1 - opacity, FlickerDuration)
                        .SetEase(easy)
                        .ToUniTask(cancellationToken: cancellationToken)
                );

                await UniTask.WhenAll(
                    lightingCanvasGroup
                        .DOFade(1, FlickerDuration)
                        .SetEase(easy)
                        .ToUniTask(cancellationToken: cancellationToken),
                    overlay
                        .DOFade(0, FlickerDuration)
                        .SetEase(easy).
                        ToUniTask(cancellationToken: cancellationToken)
                );
                await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(.2f, .8f)),
                    cancellationToken: cancellationToken);
            }
        }
        
        public override UniTask Show()
        {
            base.Show();
            SetActive(true);
            ResetProgressBar();
            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            RemoveEventListeners();
            StopAnimation();
            base.Dispose();
        }
        
        private void RemoveEventListeners() => _disposables.Clear();

        private void StopAnimation()
        {
            if (_sequence != null && _sequence.IsActive()) 
                _sequence.Kill();
        }
    }
}