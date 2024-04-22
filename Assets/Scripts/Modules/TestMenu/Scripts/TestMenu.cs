using Services.EnergyBar;
using UnityEngine;
using VContainer;

namespace Modules.TestMenu.Scripts
{
    public class TestMenu : MonoBehaviour
    {
        private readonly TestMenuUIView _testMenuUIView;
        private readonly EnergyBarService _energyBarService;
        
        [Inject]
        public TestMenu(TestMenuUIView testMenuUIView, EnergyBarService energyBarService)
        {
            _testMenuUIView = testMenuUIView;
            Debug.Log("EnergyBarService from constructor - " + energyBarService);
            _energyBarService = energyBarService;
        }
        
        private void Start()
        {
            SetupEventListeners();
            _energyBarService.Init();
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