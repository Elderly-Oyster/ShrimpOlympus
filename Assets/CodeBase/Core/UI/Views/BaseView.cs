using CodeBase.Core.UI.Views.Animations;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Core.UI.Views
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Canvas))]
    public abstract class BaseView : MonoBehaviour, IView
    {
        [SerializeField] private BaseAnimationElement animationElement;
        private CanvasGroup _canvasGroup;
        private Canvas _canvas;
        public bool IsActive { get; private set; }

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvas = GetComponent<Canvas>();
        }

        public virtual async UniTask Show()
        {
            SetActive(true);
            if (IsActive && animationElement != null) 
                await animationElement.Show();
        }

        public virtual async UniTask Hide()
        {
            if (IsActive && animationElement != null) 
                await animationElement.Hide(); 
            SetActive(false);
        }
        
        protected void SetActive(bool isActive)
        {
            if (_canvas == null)
                _canvas = GetComponent<Canvas>();
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
            
            if (IsActive == isActive) return;
            IsActive = isActive;
            _canvas.enabled = isActive;
            _canvasGroup.alpha = isActive ? 1 : 0;
            _canvasGroup.blocksRaycasts = isActive;
            _canvasGroup.interactable = isActive;
            gameObject.SetActive(isActive);
        }
        
        public void HideInstantly() => SetActive(false);
        
        public virtual void Dispose()
        {
            if (this != null) 
                Destroy(gameObject);
        } 
    }
}