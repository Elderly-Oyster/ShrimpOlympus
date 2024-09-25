using Core.Scripts.Popup.Base;

namespace Core.Scripts.Popup.Types.FirstPopup.Scripts
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