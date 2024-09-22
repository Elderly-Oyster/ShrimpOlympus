using System;
using Cysharp.Threading.Tasks;

namespace Core.MVP
{
    public interface IScreenPresenter : IDisposable
    {
        UniTask Enter(object param);
        UniTask Execute();
        UniTask Exit();
    }
}