using CodeBase.Core.Popups;
using CodeBase.Core.Systems.PopupHub;

namespace CodeBase.Implementation.Popups.ThirdPopup
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