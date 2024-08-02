using System.Threading.Tasks;
using Core.Views.ProgressBars;
using Core.Views.UIViews;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Core.Popup.Scripts
{
    public class BasePopup : FadeUIView
    {
        [Inject][HideInInspector] public PopupRootCanvas rootCanvas;
        //[Inject] protected ISoundService soundService;
        [Inject] protected PopupHub popupHub;
        
        [SerializeField] protected Transform overlayTransform;
        [SerializeField] protected Transform spinnerTransform;
        
        public Button closeButton;
        
        private TaskCompletionSource<bool> tcs => _tcs ??= new TaskCompletionSource<bool>();

        [SerializeField] protected PopupPriority priority = PopupPriority.Medium;
        public PopupPriority Priority => priority;

        protected const float CloseTime = 0.2f;
        private bool _isClosed;
        private TaskCompletionSource<bool> _tcs;

        protected new virtual void Awake()
        {
            gameObject.SetActive(false);
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
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
            canvasGroup.alpha = 0;
            gameObject.SetActive(true);

            return base.Show();
        }

        public virtual async UniTask Close()
        {
            if(_isClosed)
                return;
            try
            {
                _isClosed = true;
                //soundService.Play(GeneralSoundTypes.GeneralPopupClose).Forget();
                await base.Hide();
                await UniTask.WaitForSeconds(CloseTime);
                transform.DOKill();
            }
            finally
            {
                tcs?.TrySetResult(true);
                Destroy(gameObject);
                popupHub.NotifyPopupClosed(); 
            }
        }
        
        public Task<bool> WaitForCompletion() => tcs.Task;

        private void OnDestroy() => _isClosed = true;
    }
}
