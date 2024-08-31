using System.Threading.Tasks;
using CodeBase.Core.UI.Views.Animations;
using Core.Views.ProgressBars;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Core.Popup.Scripts
{
    public class BasePopup : MonoBehaviour
    {
        [Inject][HideInInspector] public PopupRootCanvas rootCanvas;
        //[Inject] protected ISoundService soundService;
        [Inject] protected PopupHub PopupHub;
        
        [SerializeField] protected Transform overlayTransform;
        [SerializeField] protected Transform spinnerTransform;
        [SerializeField] private BaseAnimationElement animationElement;
        
        public Button closeButton;
        
        private TaskCompletionSource<bool> tcs => _tcs ??= new TaskCompletionSource<bool>();

        [SerializeField] protected PopupPriority priority = PopupPriority.Medium;
        public PopupPriority Priority => priority;

        private bool _isClosed;
        private TaskCompletionSource<bool> _tcs;

        protected new virtual void Awake()
        {
            gameObject.SetActive(false);
     
            if (closeButton != null)
                closeButton.onClick.AddListener(() => Close().Forget());
        }
        
        protected void ShowSpinner()
        {
            overlayTransform.gameObject.SetActive(true);
            spinnerTransform.gameObject.SetActive(true);
            spinnerTransform.DORotate(new Vector3(0, 0, -360), 1.5f, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Incremental);
        }
        
        protected void HideSpinner()
        {
            overlayTransform.gameObject.SetActive(false);
            spinnerTransform.gameObject.SetActive(false);
            spinnerTransform.DOKill();
        }

        public virtual UniTask Open<T>(T param)
        {
            gameObject.SetActive(true);

            return animationElement.Show();
        }

        public virtual async UniTask Close()
        {
            if(_isClosed)
                return;
            try
            {
                _isClosed = true;
                //soundService.Play(GeneralSoundTypes.GeneralPopupClose).Forget();
                await animationElement.Hide();
                transform.DOKill();
            }
            finally
            {
                tcs?.TrySetResult(true);
                Destroy(gameObject);
                PopupHub.NotifyPopupClosed(); 
            }
        }
        
        public Task<bool> WaitForCompletion() => tcs.Task;

        private void OnDestroy() => _isClosed = true;
    }
}
