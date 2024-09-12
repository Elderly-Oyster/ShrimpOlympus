using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Core.Views.UIViews.Animations
{
    [RequireComponent(typeof(CanvasGroup))]
    public class BubbleFadeAnimation : BaseAnimationElement
    {
        [SerializeField] protected CanvasGroup canvasGroup;

        [Header("Animation Parameters")]
        [SerializeField] private float scaleUpFactor = 1.1f;
        [SerializeField] private float scaleDuration = 0.25f;
        [SerializeField] private float fadeDuration = 0.25f;
        
        protected void Awake()
        {
            if (!canvasGroup)
                TryGetComponent(out canvasGroup);
            if (!canvasGroup) 
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        public override async UniTask Show()
        {
            transform.localScale = Vector3.zero;

            Sequence = DOTween.Sequence();
            Sequence
                .Append(transform.DOScale(scaleUpFactor, scaleDuration / 2))
                .Join(canvasGroup.DOFade(1, fadeDuration))
                .Append(transform.DOScale(1, scaleDuration / 2));

            await Sequence;
        }

        public override async UniTask Hide()
        {
            Sequence = DOTween.Sequence();
            Sequence
                .Append(transform.DOScale(scaleUpFactor, scaleDuration / 2))
                .Join(canvasGroup.DOFade(0, fadeDuration))
                .Append(transform.DOScale(0, scaleDuration / 2));

            await Sequence;
        }
    }
}