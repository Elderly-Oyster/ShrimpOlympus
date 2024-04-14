using TMPro;
using UnityEngine;

namespace Core.UI.EnergyBar
{
    public class InfinityEnergyView : MonoBehaviour
    {
        public TMP_Text amountText;
        
        public bool IsInfinity
        {
            get;
            private set;
        }
        
        public void SetInfinity()
        {
            IsInfinity = true;
            amountText.text = "∞";
            amountText.fontSize = 75;
            amountText.alignment = TextAlignmentOptions.Center;
        }
    }
}