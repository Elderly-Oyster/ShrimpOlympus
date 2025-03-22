using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameData;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.BaseClasses.Parcels;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Containers;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers.ContainerManagerLogic
{
    public class ContainerManager : MonoBehaviour
    {
        [SerializeField] private ContainersAddressableKeys containersAddressableKeys;
        [SerializeField] private List<ContainerHolder> containerHolders;
        [SerializeField] private List<ParcelType> parcelTypesForContainersToBeBought;
        
        private GameObject _cachedContainerModel;
        private ParcelType _parcelTypeToAssign;
        private bool _isInitialized;
        private Mediator _mediator;
        private readonly ReactiveProperty<List<ContainerHolder>> _containerHolder = new();
        public ReadOnlyReactiveProperty<List<ContainerHolder>> ContainerHoldersList => _containerHolder;

        [Inject] public void Construct(Mediator mediator) => _mediator = mediator;

        public void Initialize(List<ContainerHoldersData> containerHoldersData)
        { 
            _containerHolder.Value = containerHolders;
            var containersList = containerHoldersData
                .Find(c => c.hasInitializedContainer);
            if (containersList == null)
                StartWarmUpOfContainer();
            else
            {
                var containersToInitialize = containerHoldersData.
                    FindAll(c => c.hasInitializedContainer);
                foreach (var container in containersToInitialize)
                {
                    parcelTypesForContainersToBeBought.Remove(container.parcelType);
                    PreloadContainerPrefab(container.parcelType);
                }
            }
        }

        public void StartWarmUpOfContainer()
        {
            var containerModelToWarmUp = parcelTypesForContainersToBeBought[0];
            PreloadContainerPrefab(containerModelToWarmUp);
            parcelTypesForContainersToBeBought.Remove(containerModelToWarmUp);
        }

        private async Task InitializeContainer()
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
            
            await _mediator.Send(new ContainerManagerOperations.AddNewContainerCommand(_containerHolder.Value));

            _containerHolder.Value = new List<ContainerHolder>(containerHolders);
            _cachedContainerModel = null;
            _parcelTypeToAssign = ParcelType.None;
        }


        private void PreloadContainerPrefab(ParcelType parcelType)
        {
            var addressableKey = containersAddressableKeys.GetContainerKey(parcelType);
            if (string.IsNullOrEmpty(addressableKey)) return;
        
            Addressables.LoadAssetAsync<GameObject>(addressableKey).Completed += handle 
                => OnModelLoaded(handle, parcelType);
        }

        private void OnModelLoaded(AsyncOperationHandle<GameObject> handle, ParcelType parcelType)
        {
            Debug.Log("OnModelLoaded");
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _cachedContainerModel = handle.Result;
                _parcelTypeToAssign = parcelType;
                _ = InitializeContainer();
            }
            else
                Debug.LogError("Failed to load service building model");
        }
    }
}
    
