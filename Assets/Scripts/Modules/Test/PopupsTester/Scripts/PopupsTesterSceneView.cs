using Core.Views;
using UnityEngine;
using System.Collections.Generic;

namespace Modules.Test.PopupsTester.Scripts
{
    public class PopupsTesterSceneView : UIView
    {
        private List<TestButtonView> _testButtonViews;
        [SerializeField] public Transform buttonsParent;

        public void SetupEventListeners(List<TestButtonView> testButtons) => _testButtonViews = testButtons;

        public void RemoveEventListeners()
        {
            foreach (var testButton in _testButtonViews)
                testButton.button.onClick.RemoveAllListeners();
        }
    }
}