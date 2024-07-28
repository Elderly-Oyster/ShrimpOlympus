using UnityEngine;
using UnityEngine.UI;

namespace Core.Views
{
    public class RootCanvas : MonoBehaviour
    {
        private float? _scaleFactor;
        
        [SerializeField] private CanvasScaler canvasScaler;
        [SerializeField] private Transform contentParent;
        [SerializeField] private Transform popupParent;
        [SerializeField] private Transform rewardParent;
        [SerializeField] private Transform notificationParent;
        [SerializeField] private Camera uiCamera;
        
        public float GetScaleFactor()
        {
            if(!_scaleFactor.HasValue)
            {
                _scaleFactor = 1f;
                if (canvasScaler.screenMatchMode == CanvasScaler.ScreenMatchMode.MatchWidthOrHeight)
                {
                    var logWidth = Mathf.Log(Screen.width / canvasScaler.referenceResolution.x, 2);
                    var logHeight = Mathf.Log(Screen.height / canvasScaler.referenceResolution.y, 2);
                    var logWeightedAverage = Mathf.Lerp(logWidth, logHeight, canvasScaler.matchWidthOrHeight);
                    _scaleFactor = Mathf.Pow(2, logWeightedAverage);
                }
            }

            return _scaleFactor.Value;
        }
        
        public Transform PopupParent => popupParent;
        
        public Transform ContentParent => contentParent;
        
        public Transform RewardParent => rewardParent;
        
        public Transform NotificationParent => notificationParent;
        
        public Camera UICamera => uiCamera;
        
        public Rect GetScreenSpaceBounds(RectTransform rectTransform)
        {
            var worldCorners = new Vector3[4];
            rectTransform.GetWorldCorners(worldCorners);

            var screenBottomLeft = UICamera.WorldToScreenPoint(worldCorners[0]);
            var screenTopRight = UICamera.WorldToScreenPoint(worldCorners[2]);

            var x = screenBottomLeft.x;
            var y = screenBottomLeft.y;
            var width = screenTopRight.x - screenBottomLeft.x;
            var height = screenTopRight.y - screenBottomLeft.y;

            return new Rect(x, y, width, height);
        }
    }
}