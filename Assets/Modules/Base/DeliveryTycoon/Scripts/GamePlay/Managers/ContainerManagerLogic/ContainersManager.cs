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
using static Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers.ContainerManagerLogic.ContainerManagerOperations;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers.ContainerManagerLogic
{
    public class ContainerManager : MonoBehaviour
    {
        [SerializeField] private ContainersAddressableKeys containersAddressableKeys;
        private List<ContainerHolder> _containerHolders;
        private List<ParcelType> _parcelTypesForContainersToBeBought = new();

        private IMediator _mediator;
        private LoadingServiceProvider _loadingServiceProvider;
        
        private GameObject _cachedContainerModel;
        private ParcelType _parcelTypeToAssign;
        private bool _isInitialized;
        private bool _warmUpInitialized;
        
        private readonly ReactiveProperty<List<ContainerHolder>> _containerHolder = new();
        private readonly Subject<float> _warmUpProgress = new();
        public Observable<float> WarmUpProgress => _warmUpProgress;

        [Inject]
        public void Construct(IMediator mediator, 
            LoadingServiceProvider loadingServiceProvider,
            List<ContainerHolder> containerHolders)
        {
            _mediator = mediator;
            _loadingServiceProvider = loadingServiceProvider;
            _containerHolders = containerHolders;
            _parcelTypesForContainersToBeBought = new List<ParcelType>
            {
                ParcelType.Presents,
                ParcelType.Electronics,
                ParcelType.Furniture,
                ParcelType.MusicalInstruments
            };
            _warmUpInitialized = false;
        }

        public void Initialize(List<ContainerHoldersData> containerHoldersData)
        {
            for (int i = 0; i < _containerHolders.Count; i++)
                _containerHolders[i].SetParameters(containerHoldersData[i]);
            
            _loadingServiceProvider.RegisterCommands("ContainerManager",
                () => WarmUpContainerPrefabsAsync(containerHoldersData));
        }

        public void StartWarmUpOfContainer()
        {
            var containerModelToWarmUp = _parcelTypesForContainersToBeBought[0];
            PreloadContainerPrefab(containerModelToWarmUp);
            _parcelTypesForContainersToBeBought.Remove(containerModelToWarmUp);
        }

        private async UniTask InitializeNewContainer()
        {
            if (_cachedContainerModel == null)
                return;
            
            var firstInactiveContainHolder = _containerHolders
                .Find(c => !c.HasInitializedContainer);

            if (firstInactiveContainHolder != null)
            {
                var instance = Instantiate(_cachedContainerModel,
                    firstInactiveContainHolder.transform.position, Quaternion.identity);
                instance.transform.SetParent(firstInactiveContainHolder.transform);
                firstInactiveContainHolder.SetActiveState(_parcelTypeToAssign);
            }
            
            await _mediator.Send(new AddNewContainerCommand(_containerHolders));

            //_containerHolder.Value = new List<ContainerHolder>(containerHolders);
            _cachedContainerModel = null;
            _parcelTypeToAssign = ParcelType.None;
        }
        
        private async UniTask InitializeBoughtContainers()
        {
            if (_cachedContainerModel == null)
                return;
            
            ContainerHolder firstActiveContainHolder = _containerHolders
                .Find(c => c.HasInitializedContainer);

            if (firstActiveContainHolder != null)
            {
                Debug.Log("Initializing bought container");
                var instance = Instantiate(_cachedContainerModel,
                    firstActiveContainHolder.transform.position, Quaternion.identity);
                instance.transform.SetParent(firstActiveContainHolder.transform);
            }
            
            await _mediator.Send(new AddNewContainerCommand(_containerHolders));

            Debug.Log("Container Initialized");
            //_containerHolder.Value = new List<ContainerHolder>(containerHolders);
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
                _ = InitializeNewContainer();
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
            
            Debug.Log("Warm-up initialized.");
            return WarmUpInternalAsync(containerHoldersData);
        }

        private async UniTask WarmUpInternalAsync(List<ContainerHoldersData> containerHoldersData)
        {
            _parcelTypesForContainersToBeBought = new List<ParcelType>
            {
                ParcelType.Presents,
                ParcelType.Electronics,
                ParcelType.Furniture,
                ParcelType.MusicalInstruments
            };
            
            var containersToWarmUp = containerHoldersData
                .Where(c => c.hasInitializedContainer)
                .ToList();

            if (containersToWarmUp.Count == 0)
            {
                // Fallback to warming up the first uninitialized container
                if (_parcelTypesForContainersToBeBought.Count > 0)
                {
                    var containerModelToWarmUp = _parcelTypesForContainersToBeBought[0];
                    _parcelTypesForContainersToBeBought.RemoveAt(0); // remove before preload

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

                            await InitializeNewContainer();
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
                var test = _parcelTypesForContainersToBeBought.Count;
                _parcelTypesForContainersToBeBought.Remove(parcelType);

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

                    await InitializeBoughtContainers();
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
        
        public void ResetParameters()
        {
            _warmUpInitialized = false;
            _loadingServiceProvider.UnregisterCommands("ContainerManager");
        }
    }
}
    
