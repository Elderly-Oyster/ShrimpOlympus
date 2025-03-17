using System;
using System.Collections.Generic;
using CodeBase.Core.Gameplay.Parcels;
using R3;
using UnityEngine;

namespace CodeBase.Core.Gameplay.Cars
{
    public class BaseCarController : MonoBehaviour
    {
        [SerializeField] private GameObject parcelModel;
        
        private int _capacity = 1;
        //private int _money;
        private List<Parcel> _parcels = new();
        
        private ReactiveProperty<int> _money = new();
        private ReactiveProperty<int> _experience = new();
        public ReadOnlyReactiveProperty<int> Money => _money;
        public ReadOnlyReactiveProperty<int> Experience => _experience;

        public int Capacity => _capacity;

        //public int Money => _money;

        public void Initialize(int capacity, int money)
        {
            _capacity = capacity;
            _money.Value = money;
            parcelModel.SetActive(false);
        }

        private void AddMoney(int amount)
        {
            _money.Value += amount;
        }

        public void LoadParcel(Parcel parcel)
        {
            Debug.Log($"Loading parcel {parcel}");
            if (_parcels.Count < _capacity) 
                _parcels.Add(parcel);
            else
            {
                _parcels.RemoveAt(0);
                _parcels.Add(parcel);
            }
            
            parcelModel.SetActive(true);
        }

        public bool UnloadParcel(Parcel parcel)
        {
            if (_parcels.Contains(parcel))
            {
                Debug.Log($"Unloading parcel {parcel}");
                var parcelToUnload = _parcels.Find(p => p.ParcelType == parcel.ParcelType);
                AddMoney(parcelToUnload.DeliveryCost);
                _experience.Value = parcelToUnload.Experience;
                _parcels.Remove(parcelToUnload);
                
                if (_parcels.Count == 0)
                    parcelModel.SetActive(false);
                
                return true;
            }
            return false;
        }

        public void UpdateMoney(int amount)
        {
            _money.Value = amount;
        }
        public void UpdateCarCapacity(int newCapacity)
        {
            _capacity = newCapacity;
        }
    }
}