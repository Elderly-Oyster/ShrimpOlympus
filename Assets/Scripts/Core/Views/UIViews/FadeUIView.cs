using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Core.Views.UIViews
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeUIView : MonoBehaviour, IUIView
    {
        private const float FadeDuration = 0.4f;
        [SerializeField] protected CanvasGroup canvasGroup;

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
            canvasGroup.alpha = 0;

            await canvasGroup.DOFade(1, FadeDuration);
        }

        public virtual async UniTask Hide()
        {
            await canvasGroup.DOFade(0, FadeDuration).
                OnComplete(() => gameObject.SetActive(false));
        }
        
        public void HideInstantly()
        {
            canvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }
        
        public virtual void Dispose() => Destroy(gameObject);
    }
}