using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeBase.Services;
using Cysharp.Threading.Tasks;
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

        private Mediator _mediator;
        private LoadingServiceProvider _loadingServiceProvider;
        
        private GameObject _cachedContainerModel;
        private ParcelType _parcelTypeToAssign;
        private bool _isInitialized;
        private bool _warmUpInitialized;
        
        private readonly ReactiveProperty<List<ContainerHolder>> _containerHolder = new();
        private readonly Subject<float> _warmUpProgress = new();
        public Observable<float> WarmUpProgress => _warmUpProgress;

        [Inject] public void Construct(Mediator mediator) => _mediator = mediator;

        public void Initialize(List<ContainerHoldersData> containerHoldersData)
        { 
            _loadingServiceProvider.RegisterCommands("ContainerManager",
                () => WarmUpContainerPrefabsAsync(containerHoldersData));
            
            // _containerHolder.Value = containerHolders;
            // var containersList = containerHoldersData
            //     .Find(c => c.hasInitializedContainer);
            // if (containersList == null)
            //     StartWarmUpOfContainer();
            // else
            // {
            //     var containersToInitialize = containerHoldersData.
            //         FindAll(c => c.hasInitializedContainer);
            //     foreach (var container in containersToInitialize)
            //     {
            //         parcelTypesForContainersToBeBought.Remove(container.parcelType);
            //         PreloadContainerPrefab(container.parcelType);
            //     }
            // }
        }

        public void StartWarmUpOfContainer()
        {
            var containerModelToWarmUp = parcelTypesForContainersToBeBought[0];
            PreloadContainerPrefab(containerModelToWarmUp);
            parcelTypesForContainersToBeBought.Remove(containerModelToWarmUp);
        }

        private async Task InitializeContainer()
        {

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
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _cachedContainerModel = handle.Result;
                _parcelTypeToAssign = parcelType;
                _ = InitializeContainer();
            }
            else
                Debug.LogError("Failed to load service building model");
        }

        public UniTask WarmUpContainerPrefabsAsync(List<ContainerHoldersData> containerHoldersData)
        {
            if (_warmUpInitialized)
            {
                Debug.Log("Warm-up already completed.");
                return UniTask.CompletedTask;
            }

            return WarmUpInternalAsync(containerHoldersData);
        }

        private async UniTask WarmUpInternalAsync(List<ContainerHoldersData> containerHoldersData)
        {
            _containerHolder.Value = containerHolders;

            var containersToWarmUp = containerHoldersData
                .Where(c => c.hasInitializedContainer)
                .ToList();

            if (containersToWarmUp.Count == 0)
            {
                // Fallback to warming up the first uninitialized container
                if (parcelTypesForContainersToBeBought.Count > 0)
                {
                    var containerModelToWarmUp = parcelTypesForContainersToBeBought[0];
                    parcelTypesForContainersToBeBought.RemoveAt(0); // remove before preload

                    var addressableKey = containersAddressableKeys.GetContainerKey(containerModelToWarmUp);
                    if (!string.IsNullOrEmpty(addressableKey))
                    {
                        var handle = Addressables.LoadAssetAsync<GameObject>(addressableKey);
                        await handle.ToUniTask();

                        if (handle.Status == AsyncOperationStatus.Succeeded)
                        {
                            GameObject prefab = handle.Result;

                            _cachedContainerModel = prefab;
                            _parcelTypeToAssign = containerModelToWarmUp;

                            await InitializeContainer();
                        }
                        else
                        {
                            Debug.LogError($"Failed to load fallback container for {containerModelToWarmUp}");
                        }
                    }
                }

                _warmUpProgress.OnNext(1f);
                _warmUpProgress.OnCompleted();
                _warmUpInitialized = true;
                return;
            }
            //  Load and initialize all already-initialized containers
            int total = containersToWarmUp.Count;
            int loaded = 0;

            foreach (var container in containersToWarmUp)
            {
                var parcelType = container.parcelType;
                parcelTypesForContainersToBeBought.Remove(parcelType);

                var addressableKey = containersAddressableKeys.GetContainerKey(parcelType);
                if (string.IsNullOrEmpty(addressableKey))
                {
                    Debug.LogWarning($"No addressable key for parcel type: {parcelType}");
                    loaded++;
                    _warmUpProgress.OnNext((float)loaded / total);
                    continue;
                }

                var handle = Addressables.LoadAssetAsync<GameObject>(addressableKey);
                await handle.ToUniTask();

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    GameObject prefab = handle.Result;

                    _cachedContainerModel = prefab;
                    _parcelTypeToAssign = parcelType;

                    await InitializeContainer();
                }
                else
                {
                    Debug.LogError($"Failed to load prefab for {parcelType}");
                }

                loaded++;
                _warmUpProgress.OnNext((float)loaded / total);
            }

            _warmUpProgress.OnCompleted();
            _warmUpInitialized = true;
        }
    }
}
    
