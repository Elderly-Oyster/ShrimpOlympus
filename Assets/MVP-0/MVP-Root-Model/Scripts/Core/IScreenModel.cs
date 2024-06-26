using System;
using Cysharp.Threading.Tasks;

namespace MVP_0.MVP_Root_Model.Scripts.Core
{
    public interface IScreenModel : IModel, IDisposable
    {
        UniTask Run(object param);
        
        UniTask Stop();
    }
}