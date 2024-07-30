using Core.Popup.Scripts;

namespace Core.Popup.Popups.ThirdPopup.Scripts
{
    public class ThirdPopup : BasePopup
    {
        protected override void Awake()
        {
            priority = PopupPriority.Low; 
            base.Awake();
        }
    }
}