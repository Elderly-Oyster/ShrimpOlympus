using System;
using Cysharp.Threading.Tasks;

namespace Core.MVVM
{
    public interface IScreenPresenter : IDisposable
    {
        public bool IsNeedServices { get; protected set; }
        UniTask Enter(object param);
        UniTask Execute();
        UniTask Exit();
    }
}