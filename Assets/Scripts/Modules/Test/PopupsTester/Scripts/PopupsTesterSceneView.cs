using Core.Views;
using UnityEngine;
using System.Collections.Generic;
using CodeBase.Core.MVVM.View;
using Core.Views.UIViews;
using Cysharp.Threading.Tasks;

namespace Modules.Test.PopupsTester.Scripts
{
    public class PopupsTesterSceneView : BaseScreenView
    {
        private List<TestButtonView> _testButtonViews;
        [SerializeField] public Transform buttonsParent;
        
        private new void Awake()
        {
            base.Awake();
            HideInstantly();
        }

        public void GetPopupsButtons(List<TestButtonView> testButtons)
        {
            _testButtonViews = testButtons;
            foreach (var testButton in _testButtonViews)
            {
                testButton.HideInstantly();
                testButton.Show().Forget();
                testButton.button.onClick.AddListener(() => testButton.Show().Forget());
            }
        }

        public void RemoveEventListeners()
        {
            foreach (var testButton in _testButtonViews)
                testButton.button.onClick.RemoveAllListeners();
        }
    }
}