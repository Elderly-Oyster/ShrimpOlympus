using Cysharp.Threading.Tasks;

namespace Core
{
    public interface ISmartPresenter
    {
        UniTask Run(object param);
        
        UniTask Stop();
    }
}