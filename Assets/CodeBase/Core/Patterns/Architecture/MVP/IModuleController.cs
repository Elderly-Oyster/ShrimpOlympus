using System;
using Cysharp.Threading.Tasks;

namespace CodeBase.Core.Patterns.Architecture.MVP
{
    public interface IModuleController : IDisposable
    {
        UniTask Enter(object param);
        UniTask Execute();
        UniTask Exit();
    }
}