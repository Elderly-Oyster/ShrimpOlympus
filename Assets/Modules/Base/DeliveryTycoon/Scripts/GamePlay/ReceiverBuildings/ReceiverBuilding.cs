using System.Collections;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.BaseClasses.Cars;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.BaseClasses.Container;
using UnityEngine;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.ReceiverBuildings
{
    public class ReceiverBuilding : BaseInteractableBuilding
    {
        [SerializeField] private Collider triggerCollider;
        [SerializeField] private GameObject parcelModel;
        
        private bool _demandParcel;
        
        public bool DemandParcel => _demandParcel;
        
        protected override void Awake()
        { 
            base.Awake();
            parcelModel.gameObject.SetActive(false);
            icon.enabled = false;
            triggerCollider.enabled = false;
        }

        protected async override void CompleteAction(BaseCarController carController)
        {
            if (carController == null) return;
            
            if (await _carController.UnloadParcel(Parcel))
            {
                Debug.Log("Parcel unloaded");
                _demandParcel = false;
                icon.enabled = false;
                _carController = null;
                _taskIsInProgress = false;
                triggerCollider.enabled = false;
                StartCoroutine(ShowParcel());
            }
            HideProgressBar();
        }

        public void StartDemandParcel()
        {
            triggerCollider.enabled = true;
            _demandParcel = true;
            icon.enabled = true;
        }

        private IEnumerator ShowParcel()
        {
            parcelModel.gameObject.SetActive(true);
            yield return new WaitForSeconds(3);
            parcelModel.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}