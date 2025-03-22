using System.Collections.Generic;
using CodeBase.Core.Modules;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Modules.Base.DeliveryTycoon.Scripts.UpgradePopup
{
    public class UpgradePopupView : BaseScreenView
    {
        [SerializeField] private Button addCapacityButton;
        [SerializeField] private Button promoteCompanyButton;
        [SerializeField] private Button hireEmployeeButton;
        [SerializeField] private Button buyContainerButton;
        [SerializeField] private Button backToGameButton;
        [SerializeField] private TMP_Text addCapacityText;
        [SerializeField] private TMP_Text promoteCompanyText;
        [SerializeField] private TMP_Text hireEmployeeText;

        private List<Button> _buttons = new();
        private List<Button> _interactableButtons = new();

        public List<Button> Buttons => _buttons;

        public List<Button> InteractableButtons => _interactableButtons;

        public void SetupEventListeners(UnityAction onCloseButtonClicked, 
            UnityAction buyContainerButtonClicked, UnityAction promoteCompanyButtonClicked,
            UnityAction addCapacityButtonClicked)
        {
            buyContainerButton.onClick.AddListener(buyContainerButtonClicked);
            promoteCompanyButton.onClick.AddListener(promoteCompanyButtonClicked);
            addCapacityButton.onClick.AddListener(addCapacityButtonClicked);
            backToGameButton.onClick.AddListener(onCloseButtonClicked);
            SetInitialConfiguration();
        }

        private void SetInitialConfiguration()
        {
            _buttons.Add(promoteCompanyButton);
            _buttons.Add(buyContainerButton);
            _buttons.Add(addCapacityButton);
            _buttons.Add(hireEmployeeButton);
            addCapacityButton.interactable = false;
            promoteCompanyButton.interactable = false;
            hireEmployeeButton.interactable = false;
            buyContainerButton.interactable = false;
        }

        public void UpdateAddCapacityText(int capacity, int upgradeCapacityCost)
        {
            addCapacityText.text = $"Add Capacity up to {capacity + 1}: {upgradeCapacityCost}" ;
        }
        
        public void UpdateHireEmployeeText(int hireEmployeeCost)
        {
            hireEmployeeText.text = $"Hire Employee: {hireEmployeeCost}" ;
        }
        
        public void UpdatePromoteCompanyText(int maxNumberOfOrders, int upgradeMaxNumberOfOrdersCost)
        {
            promoteCompanyText.text = $"Promote Company. Maximum number of orders up to" +
                                      $" {maxNumberOfOrders + 1}: {upgradeMaxNumberOfOrdersCost}"; 
        }

        public override void Dispose()
        {
            base.Dispose();
            _buttons.Clear();
        }
    }
}