using System;
using Cysharp.Threading.Tasks;

namespace Core.MVVM
{
    public interface IScreenViewModel : IDisposable
    {
        UniTask Run(object param);
        
        UniTask Stop();
    }
}