using System;
using Cysharp.Threading.Tasks;

namespace Core.Scripts.MVP
{
    public interface IScreenPresenter : IDisposable
    {
        UniTask Enter(object param);
        UniTask Execute();
        UniTask Exit();
    }
}