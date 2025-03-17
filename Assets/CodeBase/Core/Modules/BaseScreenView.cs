using CodeBase.Core.Patterns.Architecture.MVP;
using CodeBase.Core.UI.Views.Animations;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Core.Modules
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Canvas))]
    public abstract class BaseScreenView : MonoBehaviour, IView
    {
        [SerializeField] private BaseAnimationElement animationElement;
        private CanvasGroup _canvasGroup;
        private Canvas _canvas;

        protected virtual void Awake()
        {
            Debug.Log("Awake works for " + gameObject.name);
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvas = GetComponent<Canvas>();
        }
        
        public virtual async UniTask Show()
        {
            SetActive(true);
            if (animationElement != null) 
                await animationElement.Show();
        }

        public virtual async UniTask Hide()
        {
            if (animationElement != null) 
                await animationElement.Hide();
            SetActive(false);
        }
        
        protected void SetActive(bool isActive)
        {
            if (_canvas == null)
                _canvas = GetComponent<Canvas>();
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
            
            if (_canvas != null)
                _canvas.enabled = isActive;
            else
            {
                Debug.LogError("Canvas not found");
            }
            
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = isActive ? 1 : 0;
                _canvasGroup.blocksRaycasts = isActive;
                _canvasGroup.interactable = isActive;
            }
            else
            {
                Debug.LogError("Canvas group not found");
            }
            
            if (transform.parent != null && !transform.parent.gameObject.activeSelf)
            {
                transform.parent.gameObject.SetActive(true);
            }

            gameObject.SetActive(isActive);
        }
        
        public void HideInstantly() => gameObject.SetActive(false);

        public virtual void Dispose()
        {
            if (this != null) 
                Destroy(gameObject);
        } 
    }
}