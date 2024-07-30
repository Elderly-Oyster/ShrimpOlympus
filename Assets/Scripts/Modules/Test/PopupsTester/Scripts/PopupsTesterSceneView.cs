using Core.Views;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Modules.Test.PopupsTester.Scripts
{
    public class PopupsTesterSceneView : UIView
    {
        [SerializeField] private Transform buttonsParent;

        public void SetupEventListeners(List<TestButtonView> buttons)
        {
            foreach (var button in buttons) 
                button.transform.SetParent(buttonsParent, false);
        }

        public void RemoveEventListeners()
        {
            foreach (Transform child in buttonsParent)
            {
                var button = child.GetComponent<TestButtonView>();
                if (button != null)
                {
                    button.button.onClick.RemoveAllListeners();
                }
            }
        }
    }
}