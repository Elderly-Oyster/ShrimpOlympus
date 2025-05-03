using System;
using Cysharp.Threading.Tasks;

namespace CodeBase.Core.Modules
{
    public interface IScreenPresenter : IDisposable
    {
        UniTask Enter(object param);
        UniTask Execute();
        UniTask Exit();
    }
}