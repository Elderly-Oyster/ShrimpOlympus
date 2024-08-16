using System;
using Cysharp.Threading.Tasks;

namespace Core.MVVM
{
    public interface IScreenPresenter : IDisposable
    {
        UniTask Run(object param);
        
        UniTask Stop();
    }
}