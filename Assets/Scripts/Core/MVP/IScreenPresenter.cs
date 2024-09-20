using System;
using Cysharp.Threading.Tasks;

namespace Core.MVVM
{
    public interface IScreenPresenter : IDisposable
    {
        UniTask Enter(object param);
        UniTask Execute();
        UniTask Exit();
    }
}