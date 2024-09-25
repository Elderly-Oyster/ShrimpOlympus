using Core.Scripts.Popup.Base;

namespace Core.Scripts.Popup.Types.ThirdPopup.Scripts
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