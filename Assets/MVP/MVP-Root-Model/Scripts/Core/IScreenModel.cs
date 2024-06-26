using System;
using Cysharp.Threading.Tasks;

namespace MVP.MVP_Root_Model.Scripts.Core
{
    public interface IScreenModel : IModel, IDisposable
    {
        UniTask Run(object param);
        
        UniTask Stop();
    }
}