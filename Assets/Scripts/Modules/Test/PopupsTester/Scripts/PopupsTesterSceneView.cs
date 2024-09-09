using UnityEngine;
using System.Collections.Generic;
using CodeBase.Core.MVVM.View;
using Cysharp.Threading.Tasks;

namespace Modules.Test.PopupsTester.Scripts
{
    public class PopupsTesterSceneView : BaseScreenView
    {
        [SerializeField] public Transform buttonsParent;
        private List<TestButtonView> _testButtonViews;
        
        
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

        public override void Dispose()
        {
            base.Dispose();
            RemoveEventListeners();
        }

        private void RemoveEventListeners()
        {
            foreach (var testButton in _testButtonViews)
                testButton.button.onClick.RemoveAllListeners();
        }
    }
}