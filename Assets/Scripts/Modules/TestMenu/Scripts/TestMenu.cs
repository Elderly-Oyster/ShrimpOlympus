using Services.EnergyBar;
using UnityEngine;

namespace Modules.TestMenu.Scripts
{
    public class TestMenu : MonoBehaviour
    {
        private readonly TestMenuUIView _testMenuUIView;
        private readonly EnergyBarService _energyBarService;
        
        public TestMenu(TestMenuUIView testMenuUIView, EnergyBarService energyBarService)
        {
            _testMenuUIView = testMenuUIView;
            _energyBarService = energyBarService;
        }
        
        private void Start()
        {
            SetupEventListeners();
        }

        private void SetupEventListeners()
        {
            _testMenuUIView.getEnergyButton.onClick.AddListener(OnGetEnergy);
            _testMenuUIView.spendEnergyButton.onClick.AddListener(OnSpendEnergy);
        }
        
        private void OnGetEnergy()
        {
            _energyBarService.AddSafeUserEnergy(_energyBarService.GetEnergyBarDefaultSettings().energyPerAd);
        }

        private void OnSpendEnergy()
        {
            var cost = _energyBarService.IsEnoughEnergyToDoSmth();
            if (cost == -1) Debug.Log("Not enough energy");
        }
    }
}