using CodeBase.Core.Modules;
using Cysharp.Threading.Tasks;

namespace CodeBase.Core.Root
{
    public interface  IScreenStateMachine
    {
        public IScreenPresenter CurrentPresenter { get; }
        
        UniTaskVoid RunScreen(ScreenPresenterMap screenPresenterMap, object param = null);
    }
    
    public static class RootControllerExtension
    {
        public static UniTaskVoid RunModel(this IScreenStateMachine self, ScreenPresenterMap screenPresenterMap) => 
            self.RunScreen(screenPresenterMap);
    }
}

