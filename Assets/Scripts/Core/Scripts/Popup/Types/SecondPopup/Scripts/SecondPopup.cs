using Core.Scripts.Popup.Base;

namespace Core.Scripts.Popup.Types.SecondPopup.Scripts
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