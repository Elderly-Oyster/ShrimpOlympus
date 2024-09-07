using Core.Popup.Base;

namespace Core.Popup.Types.ThirdPopup.Scripts
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