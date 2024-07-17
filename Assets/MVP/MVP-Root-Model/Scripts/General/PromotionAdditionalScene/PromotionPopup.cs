using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MVP.MVP_Root_Model.Scripts.General.PromotionAdditionalScene
{
    public class PromotionPopup : MonoBehaviour
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private float animationDuration = 0.5f;

        private void Awake()
        {
            closeButton.onClick.AddListener(ClosePopup);
            gameObject.SetActive(false);
        }

        public async void OpenPopup()
        {
            gameObject.SetActive(true);
            await AnimateOpen();
        }

        public async void ClosePopup()
        {
            await AnimateClose();
            gameObject.SetActive(false);
        }

        private async UniTask AnimateOpen()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.zero;
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0;

            var scaleTween = rectTransform.DOScale(Vector3.one, animationDuration).SetEase(Ease.OutBack);
            var fadeTween = canvasGroup.DOFade(1, animationDuration);

            await UniTask.WhenAll(scaleTween.ToUniTask(), fadeTween.ToUniTask());
        }

        private async UniTask AnimateClose()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

            var scaleTween = rectTransform.DOScale(Vector3.zero, animationDuration).SetEase(Ease.InBack);
            var fadeTween = canvasGroup.DOFade(0, animationDuration);

            await UniTask.WhenAll(scaleTween.ToUniTask(), fadeTween.ToUniTask());
        }
    }
}