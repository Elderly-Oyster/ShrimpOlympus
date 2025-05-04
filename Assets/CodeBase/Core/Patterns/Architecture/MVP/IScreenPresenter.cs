using System;
using Cysharp.Threading.Tasks;

namespace CodeBase.Core.Patterns.Architecture.MVP
{
    public interface IScreenPresenter : IDisposable
    {
        UniTask Enter(object param);
        UniTask Exit();
    }
}