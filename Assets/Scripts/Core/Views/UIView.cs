using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Core.Views
{
    public class UIView : MonoBehaviour, IDisposable
    {
        private const float ScaleUpFactor = 1.1f;
        private const float ScaleDuration = 0.25f;
        private const float FadeDuration = 0.25f;

        public virtual async UniTask Show()
        {
            gameObject.SetActive(true);

            transform.localScale = Vector3.zero;

            var sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(ScaleUpFactor, ScaleDuration / 2))
                .Join(transform.GetComponent<CanvasGroup>().DOFade(1, FadeDuration))
                .Append(transform.DOScale(1, ScaleDuration / 2));

            await sequence;
        }

        public virtual async UniTask Hide()
        {
            var sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(ScaleUpFactor, ScaleDuration / 2))
                .Join(transform.GetComponent<CanvasGroup>().DOFade(0, FadeDuration))
                .Append(transform.DOScale(0, ScaleDuration / 2))
                .OnComplete(() => gameObject.SetActive(false));

            await sequence;
        }
        
        public void Dispose() => Destroy(gameObject);
    }
}