using Cysharp.Threading.Tasks;

namespace Core.Views.UIViews.Animations
{
    public interface IAnimationElement
    {
        UniTask Show();
        UniTask Hide();
    }
}