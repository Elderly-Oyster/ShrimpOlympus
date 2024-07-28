using System;
using Cysharp.Threading.Tasks;

namespace Scripts.Core
{
    public interface IScreenModel : IModel, IDisposable
    {
        UniTask Run(object param);
        
        UniTask Stop();
    }
}