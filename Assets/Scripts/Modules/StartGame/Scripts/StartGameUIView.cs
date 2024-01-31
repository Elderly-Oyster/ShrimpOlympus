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
    public class StartGameUIView : UIView
    {
        [SerializeField] private Button continueButton;
        
        [SerializeField] private TMP_Text progressValue;
        [SerializeField] private TMP_Text progressText;
        [SerializeField] private Image progressBar;

        [SerializeField] private CanvasGroup progressBarCanvasGroup;
        [SerializeField] private CanvasGroup lightingCanvasGroup; // Тут не реализовывал, мб позже

        [SerializeField] private Image stuffImage;
        [SerializeField] private Image overlay;

        [SerializeField] private TMP_Text splashTooltipsText;
        [SerializeField] private TMP_Text versionText;
        
        [SerializeField] private string[] tooltips;

        private readonly CancellationTokenSource _cancellationTokenSource = new();
        
        public float duration = .2f;

        
        private void Awake() => ResetProgressBar();
        
        private void ResetProgressBar() => progressBar.fillAmount = 0;

        private void Start()
        {
            versionText.text = Application.version;
            splashTooltipsText.transform.parent.gameObject.SetActive(true);
            ShowTooltips(_cancellationTokenSource.Token).Forget();
        }
        
        public override UniTask Show() => UniTask.CompletedTask;

        public override UniTask Hide()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            return base.Hide();
        }
        
        public UniTask ReportProgress(float progress, string text)
        {
            progressText.text = $"Loading: {text}";
            return DOTween.To(() => progressBar.fillAmount, x =>
            {
                progressBar.fillAmount = x;
                progressValue.text = $"{(int)(x * 100)}";
                overlay.color = new Color(0, 0, 0, 1 - GetExp(x));
                lightingCanvasGroup.alpha = GetExp(x);
                stuffImage.color = new Color(1, 1, 1, Math.Max(.1f, GetExp(x)));
            }, progress, .6f).ToUniTask();
        }
        
        private float GetExp(float progress) // Тут не реализовывал, мб позже
        {
            var expValue = Math.Exp(progress);
            var minExp = Math.Exp(0);
            var maxExp = Math.Exp(1);

            return (float)((expValue - minExp) / (maxExp - minExp));
        }
        
        public void ReportText(string text) => progressText.text = text; // Если можно резделить инициализацию сервиса
        
        public UniTask WaitButton() => continueButton.OnClickAsync();

        private async UniTaskVoid ShowTooltips(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;
            
            var index = Random.Range(0, tooltips.Length - 1);
            while (!cancellationToken.IsCancellationRequested)
            {
                splashTooltipsText.text = tooltips[index];
                await UniTask.Delay(4000, cancellationToken: cancellationToken);
                index = (index + 1) % tooltips.Length;
            }
        }
        
        public void ShowAnimations(CancellationToken cancellationToken)
        {
            progressText.text = "Tap to continue";
            progressBarCanvasGroup.DOFade(0, .5f);
            progressText.transform.DOScale(1.2f, .5f).SetLoops(-1, LoopType.Yoyo);

            StartFlickering(cancellationToken).Forget();
        }

        private async UniTaskVoid StartFlickering(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                const Ease easy = Ease.Flash;

                var opacity = Random.Range(0f, .3f);
                
                await UniTask.WhenAll(
                    lightingCanvasGroup.DOFade(opacity, duration).SetEase(easy)
                        .ToUniTask(cancellationToken: cancellationToken),
                    overlay.DOFade(1 - opacity, duration).SetEase(easy).ToUniTask(cancellationToken: cancellationToken)
                );

                await UniTask.WhenAll(
                    lightingCanvasGroup.DOFade(1, duration).SetEase(easy)
                        .ToUniTask(cancellationToken: cancellationToken),
                    overlay.DOFade(0, duration).SetEase(easy).ToUniTask(cancellationToken: cancellationToken)
                );
                await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(.2f, .8f)), cancellationToken: cancellationToken);
            }
        }
    }
}