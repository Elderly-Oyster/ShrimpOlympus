using Modules.Base.DeliveryTycoon.Scripts.GamePlay.BaseClasses.Cars;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.BaseClasses.Container;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Containers
{
    public class Container : BaseInteractableBuilding
    {
        protected override void Awake()
        {
            icon.enabled = true;
            base.Awake();
        }

        protected override void CompleteAction(BaseCarController carController)
        {
            if (carController == null) return;
            
            CarController.LoadParcel(Parcel);
            CarController = null;
            HideProgressBar();
        }
    }
}