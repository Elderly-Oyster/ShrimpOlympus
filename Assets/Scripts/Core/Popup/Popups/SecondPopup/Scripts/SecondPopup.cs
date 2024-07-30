using Core.Popup.Scripts;

namespace Core.Popup.Popups.SecondPopup.Scripts
{
    public class SecondPopup : BasePopup
    {
        protected override void Awake()
        {
            priority = PopupPriority.Medium; 
            base.Awake();
        }
    }
}