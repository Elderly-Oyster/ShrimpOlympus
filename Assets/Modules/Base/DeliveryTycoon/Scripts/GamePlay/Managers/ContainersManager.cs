using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using CodeBase.Core.Gameplay.Parcels;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.Game;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Containers;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers
{
    public class ContainerManager : MonoBehaviour
    {
        [SerializeField] private ContainersAddressableKeys containersAddressableKeys;
        [SerializeField] private List<ContainerHolder> containerHolders;
        [SerializeField] private List<ParcelType> parcelTypesForContainersToBeBought;
        
        private GameObject _cachedContainerModel;
        private ParcelType _parcelTypeToAssign;
        private bool _isInitialized;
        private ReactiveProperty<List<ContainerHolder>> _containerHolder = new();
        public ReadOnlyReactiveProperty<List<ContainerHolder>> ContainerHoldersList => _containerHolder;

        public void Initialize(List<ContainerHoldersData> containerHolders)
        { 
            _containerHolder.Value = this.containerHolders;
            var containersList = containerHolders.Find(c => c.HasInitializedContainer);
            if ( containersList == null)
            {
                StartWarmUpOfContainer();
            }
            else
            {
                var containersToInitialize = containerHolders.
                    FindAll(c => c.HasInitializedContainer);
                for (int i = 0; i < containersToInitialize.Count; i++)
                {
                    parcelTypesForContainersToBeBought.Remove(containersToInitialize[i].ParcelType);
                    PreloadContainerPrefab(containersToInitialize[i].ParcelType);
                }
            }
        }

        public void StartWarmUpOfContainer()
        {
            var containerModelToWarmUp = parcelTypesForContainersToBeBought[0];
            PreloadContainerPrefab(containerModelToWarmUp);
            parcelTypesForContainersToBeBought.Remove(containerModelToWarmUp);
        }

        public void InitializeServiceBuilding()
        {
            Debug.Log("Initializing service building");

            if (_cachedContainerModel == null)
                return;
            
            ContainerHolder firstInactiveContainHolder = containerHolders
                .Find(c => !c.HasInitializedContainer);

            if (firstInactiveContainHolder != null)
            {
                var instance = Instantiate(_cachedContainerModel,
                    firstInactiveContainHolder.transform.position, Quaternion.identity);
                instance.transform.SetParent(transform);
                firstInactiveContainHolder.SetActiveState(_parcelTypeToAssign);
                Debug.Log($"     Assigned ParcelType {_parcelTypeToAssign} to {firstInactiveContainHolder.name}");
            }

            _containerHolder.Value = containerHolders;
            _cachedContainerModel = null;
            _parcelTypeToAssign = ParcelType.None;
        }


        private void PreloadContainerPrefab(ParcelType parcelType)
        {
            var addressableKey = containersAddressableKeys.GetContainerKey(parcelType);
            if (string.IsNullOrEmpty(addressableKey)) return;
        
            Addressables.LoadAssetAsync<GameObject>(addressableKey).Completed += handle =>
            {
                OnModelLoaded(handle, parcelType);
            };
        }

        private void OnModelLoaded(AsyncOperationHandle<GameObject> handle, ParcelType parcelType)
        {
            Debug.Log("OnModelLoaded");
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _cachedContainerModel = handle.Result;
                _parcelTypeToAssign = parcelType;
                InitializeServiceBuilding();
            }
            else
                Debug.LogError($"Failed to load service building model");
        }
    }
}
    
