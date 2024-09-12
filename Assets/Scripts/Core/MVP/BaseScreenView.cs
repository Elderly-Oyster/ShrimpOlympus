using Core.Views.UIViews;
using Core.Views.UIViews.Animations;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.MVP
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Canvas))]
    public abstract class BaseScreenView : MonoBehaviour, IUIView
    {
        [SerializeField] private BaseAnimationElement animationElement;
        private CanvasGroup _canvasGroup;
        private Canvas _canvas;

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvas = GetComponent<Canvas>();
        }

        public virtual async UniTask Show()
        {
            SetActive(true);
            if (animationElement != null) await animationElement.Show();
        }

        public virtual async UniTask Hide()
        {
            if (animationElement != null) await animationElement.Hide();
            SetActive(false);
        }
        
        protected void SetActive(bool isActive)
        {
            _canvas.enabled = isActive;
            _canvasGroup.alpha = isActive ? 1 : 0;
            _canvasGroup.blocksRaycasts = isActive;
            _canvasGroup.interactable = isActive;
            
            gameObject.SetActive(isActive);
        }
        
        public void HideInstantly() => gameObject.SetActive(false);

        public virtual void Dispose()
        {
            if (this != null) Destroy(gameObject);
        } 
    }
}