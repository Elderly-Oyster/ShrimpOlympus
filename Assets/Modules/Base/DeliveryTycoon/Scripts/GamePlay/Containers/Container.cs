using CodeBase.Core.Gameplay.Cars;
using CodeBase.Core.Gameplay.Container;

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
            
            _carController.LoadParcel(Parcel);
            _carController = null;
            _taskIsInProgress = false;
             HideProgressBar();
        }
    }
}