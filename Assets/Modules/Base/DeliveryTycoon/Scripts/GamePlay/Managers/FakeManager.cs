using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Services;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers
{
    public class FakeManager : MonoBehaviour
    {
        [SerializeField] private List<string> buildingAddressableNames; // e.g., "Building_1", "Building_2", etc.
        [SerializeField] private Transform buildingParent;
        [SerializeField] private List<bool> initializedBuildingsFlags;

        private LoadingServiceProvider _loadingServiceProvider;

        private readonly Subject<float> _loadingProgress = new();
        public Observable<float> LoadingProgress => _loadingProgress;

        private bool _isInitialized;

        [Inject]
        public void Construct(LoadingServiceProvider loadingServiceProvider)
        {
            _loadingServiceProvider = loadingServiceProvider;
        }

        public void Initialize() =>
            _loadingServiceProvider.RegisterCommands("FakeManager", LoadAndInitializeBuildingsAsync);

        public UniTask LoadAndInitializeBuildingsAsync()
            {
                if (_isInitialized)
                {
                    Debug.Log("Buildings already initialized.");
                    return UniTask.CompletedTask;
                }

                return LoadInternalAsync();
            }

            private async UniTask LoadInternalAsync()
            {
                var buildingsToLoad = buildingAddressableNames;

                int total = buildingsToLoad.Count;
                int loaded = 0;

                if (total == 0)
                {
                    _loadingProgress.OnNext(1f);
                    _loadingProgress.OnCompleted();
                    _isInitialized = true;
                    return;
                }

                foreach (var addressableKey in buildingsToLoad)
                {
                    var handle = Addressables.LoadAssetAsync<GameObject>(addressableKey);
                    await handle.ToUniTask();

                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        GameObject prefab = handle.Result;
                        GameObject instance = UnityEngine.Object.Instantiate(prefab, GetSpawnPosition(loaded), Quaternion.identity, buildingParent);
                    }
                    else
                    {
                        Debug.LogError($"Failed to load building prefab: {addressableKey}");
                    }

                    loaded++;
                    _loadingProgress.OnNext((float)loaded / total);
                }

                _loadingProgress.OnCompleted();
                _isInitialized = true;
            }

            private Vector3 GetSpawnPosition(int index)
            {
                return new Vector3(index * 5f, 0f, 0f); // Simple horizontal layout
            }
        }
    }

