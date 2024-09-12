using UnityEngine;

namespace Core.Views.ProgressBars
{
    public class PopupCanvas : ScreensCanvas
    {
        [SerializeField] private Transform popupParent;
        
        public Transform PopupParent => popupParent;
    }
}