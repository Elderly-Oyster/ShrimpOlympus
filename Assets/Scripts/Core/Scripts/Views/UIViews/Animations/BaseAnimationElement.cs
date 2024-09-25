using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Core.Scripts.Views.UIViews.Animations
{
    public abstract class BaseAnimationElement : MonoBehaviour, IAnimationElement
    {
        protected Sequence Sequence;
        
        public abstract UniTask Show();
        public abstract UniTask Hide();
        
        private void OnDisable() => Sequence.Kill();
    }
}