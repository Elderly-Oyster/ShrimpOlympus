using System;
using Cysharp.Threading.Tasks;

namespace Core.MVP
{
    public interface IUIView : IDisposable
    {
        public UniTask Show();

        public UniTask Hide();

        public void HideInstantly();
    }
}