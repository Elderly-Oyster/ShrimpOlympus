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

        /// <summary>
        /// Calculates and returns the scaling factor for the UI canvas based on the screen resolution and 
        /// the settings of the associated <see cref="CanvasScaler"/>.
        /// This factor determines how UI elements should scale to match the screen size.
        /// </summary>
        /// <returns>
        /// A float representing the scale factor. If the canvas is set to 
        /// <see cref="CanvasScaler.ScreenMatchMode.MatchWidthOrHeight"/>, the scale factor is computed
        /// based on the weighted average of the screen width and height relative to the reference resolution.
        /// Otherwise, a default scale factor of 1 is returned.
        /// </returns>
        public float GetScaleFactor()
        {
            //If _scaleFactor already has a value, the method returns it immediately, skipping the computation
            if (_scaleFactor.HasValue) return _scaleFactor.Value;
            _scaleFactor = 1f; //If _scaleFactor hasn’t been calculated, it is initialized to 1f as a default value
            
            //If the CanvasScaler is set to MatchWidthOrHeight, the method calculates a scale factor based on the screen resolution
            // MatchWidthOrHeight:
            // A value between 0 and 1 in the CanvasScaler that determines whether to match width (0),
            // height (1), or a weighted average of both
            if (canvasScaler.screenMatchMode == CanvasScaler.ScreenMatchMode.MatchWidthOrHeight)
            {
                var logWidth = Mathf.Log(Screen.width / canvasScaler.referenceResolution.x, 2);
                var logHeight = Mathf.Log(Screen.height / canvasScaler.referenceResolution.y, 2);
                var logWeightedAverage = Mathf.Lerp(logWidth, logHeight, canvasScaler.matchWidthOrHeight);
                _scaleFactor = Mathf.Pow(2, logWeightedAverage);
            }

            return _scaleFactor.Value;
        }
        
        /// <summary>
        /// Calculates and returns the screen-space bounding rectangle of a given <see cref="RectTransform"/>.
        /// The resulting rectangle is defined in pixel coordinates relative to the screen.
        /// </summary>
        /// <param name="rectTransform">The <see cref="RectTransform"/> whose screen-space bounds are to be calculated.</param>
        /// <returns>
        /// A <see cref="Rect"/> representing the bounding rectangle in screen space, where:
        /// - The bottom-left corner is given by the rectangle's (x, y) position.
        /// - The width and height are measured in screen pixels.
        /// </returns>
        public Rect GetScreenSpaceBounds(RectTransform rectTransform)
        {
            //Populates the worldCorners array with the 3D world-space positions of the RectTransform's four corners.
            var worldCorners = new Vector3[4];
            //worldCorners now contains the global positions of the RectTransform's bounds in world space
            rectTransform.GetWorldCorners(worldCorners);

            var screenBottomLeft = UICamera.WorldToScreenPoint(worldCorners[0]);
            var screenTopRight = UICamera.WorldToScreenPoint(worldCorners[2]);

            var x = screenBottomLeft.x;
            var y = screenBottomLeft.y;
            var width = screenTopRight.x - screenBottomLeft.x;
            var height = screenTopRight.y - screenBottomLeft.y;
            
            // Returns a Rect object representing the screen-space bounds:
            // (x, y) specifies the position of the bottom-left corner in screen coordinates.
            //     width and height specify the dimensions in screen pixels.
            return new Rect(x, y, width, height);
        }
    }
}