using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Views.UIViews.Animations
{
    public class FlickerAnimation
    {
        private readonly CanvasGroup _lightingCanvasGroup;
        private readonly Image _overlay;
        private const float FlickerDuration = 0.2f;

        public FlickerAnimation(CanvasGroup lightingCanvasGroup, Image overlay)
        {
            _lightingCanvasGroup = lightingCanvasGroup;
            _overlay = overlay;
        }

        public async UniTaskVoid StartFlickering(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                const Ease easy = Ease.Flash;
                var opacity = UnityEngine.Random.Range(0f, 0.3f);

                await UniTask.WhenAll(
                    _lightingCanvasGroup
                        .DOFade(opacity, FlickerDuration)
                        .SetEase(easy)
                        .ToUniTask(cancellationToken: cancellationToken),
                    _overlay
                        .DOFade(1 - opacity, FlickerDuration)
                        .SetEase(easy)
                        .ToUniTask(cancellationToken: cancellationToken)
                );

                await UniTask.WhenAll(
                    _lightingCanvasGroup
                        .DOFade(1, FlickerDuration)
                        .SetEase(easy)
                        .ToUniTask(cancellationToken: cancellationToken),
                    _overlay
                        .DOFade(0, FlickerDuration)
                        .SetEase(easy)
                        .ToUniTask(cancellationToken: cancellationToken)
                );

                await UniTask.Delay(TimeSpan.FromSeconds(UnityEngine.Random.Range(0.2f, 0.8f)),
                    cancellationToken: cancellationToken);
            }
        }
    }
}