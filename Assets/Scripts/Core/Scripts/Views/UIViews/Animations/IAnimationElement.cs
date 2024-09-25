using Cysharp.Threading.Tasks;

namespace Core.Scripts.Views.UIViews.Animations
{
    public interface IAnimationElement
    {
        UniTask Show();
        UniTask Hide();
    }
}