using System.Threading;
using Core.MVP;
using Core.Views.UIViews.Animations;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.Additional.LoadingSplashScreen.Scripts
{
    public class LoadingSplashScreenView : BaseScreenView
    {
        [Header("Progress UI Components")]
        [SerializeField] private CanvasGroup progressBarCanvasGroup;
        [SerializeField] private TMP_Text progressValueText;
        [SerializeField] private TMP_Text progressText;
        [SerializeField] private Image progressBar;

        [Header("Dynamic UI Visuals")]
        [SerializeField] private Image stuffImage;
        [SerializeField] private Image overlay;

        [Header("Splash Screen Components")]
        [SerializeField] private TMP_Text splashTooltipsText;

        private Sequence _sequence;

        private const float ProgressBarAnimDuration = 0.5f;

        private void Start()
        {
            splashTooltipsText.transform.parent.gameObject.SetActive(true);
        }
        
        public UniTask ReportProgress(float expProgress, string progressStatus)
        {
            progressText.text = progressStatus;
            return DOTween.To(() => progressBar.fillAmount, x =>
            {
                progressBar.fillAmount = x;
                progressValueText.text = $"{(int)(x * 100)}%";
                overlay.color = new Color(0, 0, 0, 1 - expProgress);
                stuffImage.color = new Color(1, 1, 1, Mathf.Max(.1f, expProgress));
            }, expProgress, 1f).ToUniTask();
        }

        public void SetTooltipText(string text) => splashTooltipsText.text = text;

        public override UniTask Show()
        {
            base.Show().Forget();
            SetActive(true);
            progressBar.fillAmount = 0;
            return UniTask.CompletedTask;
        }

        public override void Dispose()
        {
            StopAnimation();
            base.Dispose();
        }

        private void StopAnimation()
        {
            if (_sequence == null || !_sequence.IsActive()) return;
            _sequence.Kill();
            _sequence = null;
        }
    }
}
