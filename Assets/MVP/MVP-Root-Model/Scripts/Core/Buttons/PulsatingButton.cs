using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MVP.MVP_Root_Model.Scripts.Core.Buttons
{
    public class PulsatingButton : MonoBehaviour
    {
        public Button pulsatingButton;
        private Sequence _animationSequence;

        public void PlayAnimation()
        {
            _animationSequence = DOTween.Sequence();
            _animationSequence.Append(pulsatingButton.transform.DOScale(1.2f, 0.3f).SetEase(Ease.OutQuad))
                .Append(pulsatingButton.transform.DOScale(1.0f, 0.3f).SetEase(Ease.OutQuad))
                .SetLoops(-1, LoopType.Restart);

            _animationSequence.Play();
        }

        public void StopAnimation()
        {
            if (_animationSequence != null && _animationSequence.IsActive())
            {
                _animationSequence.Kill();
                pulsatingButton.transform.localScale = Vector3.one; // Сброс масштаба до исходного
            }
        }
    }
}