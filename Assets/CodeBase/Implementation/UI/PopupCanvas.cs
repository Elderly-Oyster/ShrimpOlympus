using CodeBase.Core.Systems.PopupHub;
using UnityEngine;

namespace CodeBase.Implementation.UI
{
    //TODO Действия аналогичные с ScreenCanvas, или придумать что-то более подходящее
    public class PopupCanvas : ScreenCanvas, IPopupCanvas
    {
        [SerializeField] private Transform popupParent;
        
        public Transform PopupParent => popupParent;
    }
}