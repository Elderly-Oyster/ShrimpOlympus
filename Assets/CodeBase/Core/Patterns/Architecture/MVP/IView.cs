using System;
using Cysharp.Threading.Tasks;

namespace CodeBase.Core.Patterns.Architecture.MVP
{
    public interface IView : IDisposable
    {
        public UniTask Show();

        public UniTask Hide();

        public void HideInstantly();
    }
}