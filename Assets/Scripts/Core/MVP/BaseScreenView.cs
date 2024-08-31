using CodeBase.Core.UI.Views.Animations;
using Core.Views.UIViews;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Core.MVVM.View
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class BaseScreenView : MonoBehaviour, IUIView
    {
        [SerializeField] private BaseAnimationElement animationElement;
        private CanvasGroup _canvasGroup;

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
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