using System;
using Cysharp.Threading.Tasks;

namespace Core
{
    public interface IScreenPresenter : IPresenter, IDisposable
    {
        UniTask Run(object param);
        
        UniTask Stop();
    }
}