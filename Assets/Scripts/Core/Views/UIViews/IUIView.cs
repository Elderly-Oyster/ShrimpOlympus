using System;
using Cysharp.Threading.Tasks;

namespace Core.Views.UIViews
{
    public interface IUIView : IDisposable
    {
        public UniTask Show();

        public UniTask Hide();

        public void HideInstantly();
    }
}