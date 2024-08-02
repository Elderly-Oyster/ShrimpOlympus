using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Core.Views.UIViews
{
    [RequireComponent(typeof(CanvasGroup))]
    public class BubbleFadeUIView : MonoBehaviour, IUIView
    {
        [SerializeField] protected CanvasGroup canvasGroup;

        private const float ScaleUpFactor = 1.1f;
        private const float ScaleDuration = 0.4f;
        private const float FadeDuration = 0.4f;

        protected void Awake()
        {
            if (!canvasGroup)
                canvasGroup = GetComponent<CanvasGroup>();
            if (!canvasGroup) 
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        public virtual async UniTask Show()
        {
            gameObject.SetActive(true);

            transform.localScale = Vector3.zero;

            var sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(ScaleUpFactor, ScaleDuration / 2))
                .Join(canvasGroup.DOFade(1, FadeDuration))
                .Append(transform.DOScale(1, ScaleDuration / 2));

            await sequence;
        }

        public virtual async UniTask Hide()
        {
            var sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(ScaleUpFactor, ScaleDuration / 2))
                .Join(canvasGroup.DOFade(0, FadeDuration))
                .Append(transform.DOScale(0, ScaleDuration / 2))
                .OnComplete(() => gameObject.SetActive(false));

            await sequence;
        }

        public void HideInstantly()
        {
            transform.localScale = Vector3.zero;
            canvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }

        public void Dispose() => Destroy(gameObject);
    }
}