using UnityEngine;
using UnityEngine.UI;

namespace Core.Scripts.Views
{
    public class ScreensCanvas : MonoBehaviour
    {
        [SerializeField] private CanvasScaler canvasScaler;
        [SerializeField] private Camera uiCamera;
        
        public Camera UICamera => uiCamera;
        private float? _scaleFactor;


        public float GetScaleFactor()
        {
            if (_scaleFactor.HasValue) return _scaleFactor.Value;
            _scaleFactor = 1f;
            
            if (canvasScaler.screenMatchMode == CanvasScaler.ScreenMatchMode.MatchWidthOrHeight)
            {
                var logWidth = Mathf.Log(Screen.width / canvasScaler.referenceResolution.x, 2);
                var logHeight = Mathf.Log(Screen.height / canvasScaler.referenceResolution.y, 2);
                var logWeightedAverage = Mathf.Lerp(logWidth, logHeight, canvasScaler.matchWidthOrHeight);
                _scaleFactor = Mathf.Pow(2, logWeightedAverage);
            }

            return _scaleFactor.Value;
        }
        
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