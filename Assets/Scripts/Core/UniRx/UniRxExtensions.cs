using System;
using TMPro;
using UniRx;
using UnityEngine.Events;

namespace Core.UniRx
{
    public static class UniRxExtensions
    {
        public static IObservable<string> OnValueChangedAsObservable(this TMP_InputField inputField)
        {
            return Observable.FromEvent<UnityAction<string>, string>(
                h => new UnityAction<string>(h),
                h => inputField.onValueChanged.AddListener(h),
                h => inputField.onValueChanged.RemoveListener(h)
            );
        }

        public static IObservable<int> OnValueChangedAsObservable(this TMP_Dropdown dropdown)
        {
            return Observable.FromEvent<UnityAction<int>, int>(
                h => new UnityAction<int>(h),
                h => dropdown.onValueChanged.AddListener(h),
                h => dropdown.onValueChanged.RemoveListener(h)
            );
        }
    }
}