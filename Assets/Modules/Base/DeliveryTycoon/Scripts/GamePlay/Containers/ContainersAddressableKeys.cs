using System;
using System.Collections.Generic;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.BaseClasses.Parcels;
using UnityEngine;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Containers
{
    [CreateAssetMenu(fileName = "AddressableInteractableBuildingKeys", menuName = "Configs/AddressableInteractableBuildingKeys", order = 0)]
    [Serializable]
    public class ContainersAddressableKeys : ScriptableObject
    {
        [Serializable]
        private struct AddressableKeyEntry
        {
            public ParcelType type;
            public string addressableKey;
        }
        
        [SerializeField] private List<AddressableKeyEntry> addressableKeyEntries;
        private Dictionary<ParcelType, string> keyLookup;
        
        private void OnEnable()
        {
            keyLookup = new Dictionary<ParcelType, string>();
            foreach (var entry in addressableKeyEntries)
            {
                if (!keyLookup.ContainsKey(entry.type))
                {
                    keyLookup.Add(entry.type, entry.addressableKey);
                }
                else
                {
                    Debug.LogError($"Duplicate addressable key for {entry.type} in AddressableServiceBuildingPrefabsKeys!");
                }
            }
        }
        
        public string GetContainerKey(ParcelType type)
        {
            if (keyLookup.TryGetValue(type, out string key))
                return key;
        
            Debug.LogError($"No addressable key found for {type}!");
            return string.Empty;
        }
    }
}