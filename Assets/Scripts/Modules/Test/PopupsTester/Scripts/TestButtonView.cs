using System;
using Core.Views.UIViews.Animations;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.Test.PopupsTester.Scripts
{
    public class TestButtonView : MonoBehaviour
    {
        [SerializeField] private BaseAnimationElement animationElement;
        public Button button;
        public TMP_Text label;
        private object onClick;

        public virtual async UniTask Show()
        {
            gameObject.SetActive(true);
            if (animationElement != null)
                await animationElement.Show();
        }

        public virtual async UniTask Hide()
        {
            if (animationElement != null) await animationElement.Hide();
            gameObject.SetActive(false);
        }

        public void HideInstantly() => gameObject.SetActive(false);

        public IObservable<Unit> OnClickAsObservable()
        {
            return button.onClick.AsObservable();
        }
    }
}