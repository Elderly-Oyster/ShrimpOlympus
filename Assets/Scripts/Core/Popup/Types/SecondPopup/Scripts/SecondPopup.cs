using Core.Popup.Base;

namespace Core.Popup.Types.SecondPopup.Scripts
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