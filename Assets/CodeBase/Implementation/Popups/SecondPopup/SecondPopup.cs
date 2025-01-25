using CodeBase.Core.Popups;
using CodeBase.Core.Systems.PopupHub;

namespace CodeBase.Implementation.Popups.SecondPopup
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