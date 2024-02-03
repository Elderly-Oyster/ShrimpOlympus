using System;
using System.Threading;
using Core.Views;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Modules.StartGame.Scripts
{
    public class StartGameScreenView : UIView
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
        [SerializeField] private string[] tooltips; 
        
        
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private const string TapToContinueText = "Tap to continue";
        private const float ProgressBarAnimDuration = 0.5f;
        private const float Duration = .2f;
        private const int TooltipDelay = 4000;

        private void Awake() => ResetProgressBar();
        
        private void ResetProgressBar() => progressBar.fillAmount = 0;

        private void Start()
        {
            splashTooltipsText.transform.parent.gameObject.SetActive(true);
            ShowTooltips(_cancellationTokenSource.Token).Forget();
        }
        
        public void SetVersionText(string version) => versionText.text = version;

        public override UniTask Show() => UniTask.CompletedTask;

        public override UniTask Hide()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            return base.Hide();
        }
        
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
        
        public UniTask WaitButton() => continueButton.OnClickAsync();

        private async UniTaskVoid ShowTooltips(CancellationToken cancellationToken)
        {
            try
            {
                var index = Random.Range(0, tooltips.Length - 1);
                while (!cancellationToken.IsCancellationRequested)
                {
                    splashTooltipsText.text = tooltips[index];
                    await UniTask.Delay(TooltipDelay, cancellationToken: cancellationToken);
                    index = (index + 1) % tooltips.Length;
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { Debug.LogError($"ShowTooltips Error: {ex.Message}"); }
        }
        
        public void ShowAnimations(CancellationToken cancellationToken)
        {
            progressText.text = TapToContinueText;
            progressBarCanvasGroup.DOFade(0, ProgressBarAnimDuration);
            progressText.transform.DOScale(1.2f, ProgressBarAnimDuration).
                SetLoops(-1, LoopType.Yoyo);

            StartFlickering(cancellationToken).Forget();
        }

        private async UniTaskVoid StartFlickering(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                const Ease easy = Ease.Flash;

                var opacity = Random.Range(0f, .3f);
                
                await UniTask.WhenAll(
                    lightingCanvasGroup.DOFade(opacity, Duration).SetEase(easy)
                        .ToUniTask(cancellationToken: cancellationToken),
                    overlay.DOFade(1 - opacity, Duration).SetEase(easy)
                        .ToUniTask(cancellationToken: cancellationToken)
                );

                await UniTask.WhenAll(
                    lightingCanvasGroup.DOFade(1, Duration).SetEase(easy)
                        .ToUniTask(cancellationToken: cancellationToken),
                    overlay.DOFade(0, Duration).SetEase(easy).
                        ToUniTask(cancellationToken: cancellationToken)
                );
                await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(.2f, .8f)),
                    cancellationToken: cancellationToken);
            }
        }
    }
}