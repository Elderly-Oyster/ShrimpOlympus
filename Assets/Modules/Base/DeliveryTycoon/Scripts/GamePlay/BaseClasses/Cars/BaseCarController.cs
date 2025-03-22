using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MediatR;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.BaseClasses.Parcels;
using R3;
using UnityEngine;
using VContainer;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.BaseClasses.Cars
{
    public class BaseCarController : MonoBehaviour
    {
        [SerializeField] private GameObject parcelModel;
        
        private int _capacity = 1;
        private readonly List<Parcel> _parcels = new();
        
        private readonly ReactiveProperty<int> _experience = new();
        private Mediator _mediator;
        
        public ReadOnlyReactiveProperty<int> Experience => _experience;

        public int Capacity => _capacity;

        [Inject]
        public void Construct(Mediator mediator) => _mediator = mediator;

        public void Initialize(int capacity)
        {
            _capacity = capacity;
            parcelModel.SetActive(false);
        }

        public void LoadParcel(Parcel parcel)
        {
            if (_parcels.Count == _capacity)
                _parcels.RemoveAt(0);
            
            _parcels.Add(parcel);
            
            parcelModel.SetActive(true);
        }

        public async UniTask<bool> UnloadParcel(Parcel parcel)
        {
            if (_parcels.Contains(parcel))
            {
                var parcelToUnload = _parcels.Find(p => p.ParcelType == parcel.ParcelType);
                await _mediator.Send(new BaseCarControllerOperations.MoneyObtainedCommand(parcelToUnload.DeliveryCost));
                await _mediator.Send(new BaseCarControllerOperations.ExperienceObtained(parcelToUnload.Experience));
                _parcels.Remove(parcelToUnload);
                
                if (_parcels.Count == 0)
                    parcelModel.SetActive(false);
                return true;
            }
            return false;
        }
        
        public void UpdateCarCapacity() => _capacity++;
    }
}