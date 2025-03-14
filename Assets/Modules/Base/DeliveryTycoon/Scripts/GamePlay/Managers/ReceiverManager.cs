using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Core.Gameplay.Parcels;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Containers;
using UnityEngine;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers
{
    public class 
        ReceiverManager : MonoBehaviour
    { 
        [SerializeField] private List<ReceiverBuilding> receiverBuildings = new();
        [SerializeField] private AudioClip notificationSound;
        
        private Coroutine _coroutine;
        private bool _assignReceivers = true;
        private int _maxDemandingReceivers = 1;
        private List<ReceiverBuilding> _presentDemandingReceivers = new();
        private List<ReceiverBuilding> _electronicsDemandingReceivers = new();
        private List<ReceiverBuilding> _furnitureDemandingReceivers = new();
        private List<ReceiverBuilding> _musicalInstrumentsDemandingReceivers = new();
        private List<ParcelType> _parcelTypes = new();
        private AudioSource _audioSource;
        private bool _isMusicOn;

        public int MaxDemandingReceivers => _maxDemandingReceivers;


        public void Initialize(List<ContainerHoldersData> containerHoldersData, int maxDemandingReceivers, bool isMusicPlaying)
        {
            _isMusicOn = isMusicPlaying;
            _audioSource = GetComponent<AudioSource>();
            _maxDemandingReceivers = maxDemandingReceivers;
            var containersParcelTypes = containerHoldersData.FindAll(x
                => x.HasInitializedContainer).Select(x => x.ParcelType).Distinct().ToList();
            containersParcelTypes.Remove(ParcelType.None);
            _parcelTypes = containersParcelTypes;
            SortReceiverBuildings();
            StartCoroutine(StartAssignReceivers());
        }

        public void UpdateReceiversTypes(List<ContainerHolder> containerHolders)
        {
            foreach (var containerHolder in containerHolders)
            {
                if (containerHolder.Type == ParcelType.None && containerHolder.HasInitializedContainer == false)
                    continue;
                if (!_parcelTypes.Contains(containerHolder.Type))
                {
                    Debug.Log($"Adding {containerHolder.Type} to _parcelTypes");
                    _parcelTypes.Add(containerHolder.Type);
                }
            }
        }

        public void UpdateMaxNumberOfReceivers(int maxDemandingReceivers)
        {
            _maxDemandingReceivers = maxDemandingReceivers;
        }

        private void SortReceiverBuildings()
        {
            _presentDemandingReceivers = receiverBuildings.FindAll(r => 
                r.Parcel.ParcelType == ParcelType.Presents);
            _electronicsDemandingReceivers = receiverBuildings.FindAll(r =>
                r.Parcel.ParcelType == ParcelType.Electronics);
            _furnitureDemandingReceivers = receiverBuildings.FindAll(r =>
                r.Parcel.ParcelType == ParcelType.Furniture);
            _musicalInstrumentsDemandingReceivers = receiverBuildings.FindAll(r => 
                r.Parcel.ParcelType == ParcelType.MusicalInstruments);
        }
        
        public IEnumerator StartAssignReceivers()
        {
            while (_assignReceivers)
            {
                if (receiverBuildings.Count > 0)
                {
                    List<ReceiverBuilding> potentialReceivers = new();
                    List<ReceiverBuilding> demandingReceivers = new();
                    
                    foreach (var parcelType in _parcelTypes)
                    {
                        switch (parcelType)
                        {
                            case ParcelType.Presents:
                            {
                                potentialReceivers.AddRange(_presentDemandingReceivers.FindAll(r => !r.DemandParcel));
                                demandingReceivers.AddRange(_presentDemandingReceivers.FindAll(r => r.DemandParcel));
                                break;
                            }
                            case ParcelType.Electronics:
                            {
                                potentialReceivers.AddRange(
                                    _electronicsDemandingReceivers.FindAll(r => !r.DemandParcel));
                                demandingReceivers.AddRange(
                                    _electronicsDemandingReceivers.FindAll(r => r.DemandParcel));
                                break;
                            }
                            case ParcelType.Furniture:
                            {
                                potentialReceivers.AddRange(_furnitureDemandingReceivers.FindAll(r => !r.DemandParcel));
                                demandingReceivers.AddRange(_furnitureDemandingReceivers.FindAll(r => r.DemandParcel));
                            }
                                break;
                            case ParcelType.MusicalInstruments:
                            {
                                potentialReceivers.AddRange(_musicalInstrumentsDemandingReceivers.FindAll(r => !r.DemandParcel));
                                demandingReceivers.AddRange(_musicalInstrumentsDemandingReceivers.FindAll(r => r.DemandParcel));
                                break;
                            }
                        }
                    }

                    if (demandingReceivers.Count < _maxDemandingReceivers)
                    {
                        int receiversToAssign = _maxDemandingReceivers - demandingReceivers.Count;
                        potentialReceivers = potentialReceivers.OrderBy(x => Random.value).ToList();
                        for (int i = 0; i < receiversToAssign && potentialReceivers.Count != 0; i++)
                        {
                            potentialReceivers[i].StartDemandParcel();
                        }

                        if (_isMusicOn)
                        {
                            AudioSource.PlayClipAtPoint(notificationSound, transform.TransformPoint(transform.position));
                        }
                    }
                }

                yield return new WaitForSeconds(5f);
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}