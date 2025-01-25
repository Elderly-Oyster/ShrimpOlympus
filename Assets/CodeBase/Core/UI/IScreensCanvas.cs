using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Core.Root
{
    public interface IScreensCanvas
    {
        Camera UICamera { get; }

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
        float GetScaleFactor();

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
        Rect GetScreenSpaceBounds(RectTransform rectTransform);
    }
}