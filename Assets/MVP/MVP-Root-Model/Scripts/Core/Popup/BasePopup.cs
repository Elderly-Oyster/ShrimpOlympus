using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MVP.MVP_Root_Model.Scripts.Core.Views;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace MVP.MVP_Root_Model.Scripts.Core.Popup
{
    public class BasePopup : MonoBehaviour
    {
        [Inject][HideInInspector] public RootCanvas rootCanvas;
        //[Inject] protected ISoundService soundService;
        [Inject] protected PopupHub popupHub;
        
        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] protected Transform overlayTransform;
        [SerializeField] protected Transform spinnerTransform;
        
        public Button closeButton;
        
        private TaskCompletionSource<bool> tcs => _tcs ??= new TaskCompletionSource<bool>();


        protected const float CloseTime = 0.2f;
        private bool _isClosed;
        private TaskCompletionSource<bool> _tcs;

        protected virtual void Awake()
        {
            gameObject.SetActive(false);
            if(canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
            if(closeButton != null)
                closeButton.onClick.AddListener( () => Close().Forget());
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
            return canvasGroup.DOFade(1, CloseTime).ToUniTask();
        }

        public virtual async UniTask Close()
        {
            if(_isClosed)
                return;
            try
            {
                _isClosed = true;
                //soundService.Play(GeneralSoundTypes.GeneralPopupClose).Forget();
                await canvasGroup.DOFade(0, CloseTime);
                await UniTask.WaitForSeconds(CloseTime);
                transform.DOKill();
            }
            finally
            {
                tcs?.TrySetResult(true);
                Destroy(gameObject);
            }
        }
        
        public Task<bool> WaitForCompletion() => tcs.Task;

        private void OnDestroy() => _isClosed = true;
    }
}