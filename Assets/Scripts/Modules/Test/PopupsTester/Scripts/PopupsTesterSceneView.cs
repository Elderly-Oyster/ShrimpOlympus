using UnityEngine;
using System.Collections.Generic;
using Core.MVP;
using UniRx;

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

        public void GetPopupsButtons(List<TestButtonView> testButtons, PopupsTesterScenePresenter presenter)
        {
            _testButtonViews = testButtons;
            for (int i = 0; i < _testButtonViews.Count; i++) 
                presenter.RegisterButton(_testButtonViews[i], i);
        }

        public void SetupListeners(ReactiveCommand firstPopupCommand, // Not Invoked
                ReactiveCommand secondPopupCommand,
                ReactiveCommand thirdPopupCommand)
        {
            _testButtonViews[0].OnClickAsObservable()
                .Subscribe(_ => firstPopupCommand.Execute())
                .AddTo(this);
            _testButtonViews[1].OnClickAsObservable()
                .Subscribe(_ => secondPopupCommand.Execute())
                .AddTo(this);
            _testButtonViews[2].OnClickAsObservable()
                .Subscribe(_ => thirdPopupCommand.Execute())
                .AddTo(this);
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