using System;
using Cysharp.Threading.Tasks;

namespace Core.MVVM
{
    public interface IScreenPresenter : IDisposable
    {
        public bool IsNeedServices { get; }
        UniTask Enter(object param);
        UniTask Execute();
        UniTask Exit();
    }
}