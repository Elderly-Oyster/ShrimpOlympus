using UnityEngine;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Cars
{
    public class LoadingUnloadingSlot : MonoBehaviour
    {
        [SerializeField] private GameObject carPosition;
        public bool IsUnlocked { get; set; } = false;
        public bool IsFree { get; set; } = false;

        public GameObject CarPosition => carPosition;
    }
}