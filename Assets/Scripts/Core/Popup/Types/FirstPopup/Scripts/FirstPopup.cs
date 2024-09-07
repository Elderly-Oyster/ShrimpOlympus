using Core.Popup.Base;

namespace Core.Popup.Types.FirstPopup.Scripts
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