using CodeBase.Core.Popups;
using CodeBase.Core.Systems.PopupHub;

namespace CodeBase.Implementation.Popups.FirstPopup
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