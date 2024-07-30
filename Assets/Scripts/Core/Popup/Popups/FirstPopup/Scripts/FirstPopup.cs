using Core.Popup.Scripts;

namespace Core.Popup.Popups.FirstPopup.Scripts
{
    public class FirstPopup : BasePopup
    {
        protected override void Awake()
        {
            priority = PopupPriority.High; 
            base.Awake();
        }
    }
}