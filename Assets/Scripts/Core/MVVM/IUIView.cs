using System;
using Cysharp.Threading.Tasks;

namespace Core.MVVM
{
    public interface IUIView : IDisposable
    {
        public UniTask Show();

        public UniTask Hide();

        public void HideInstantly();
    }
}