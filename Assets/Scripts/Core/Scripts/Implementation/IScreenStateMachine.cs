using Core.Scripts.MVP;
using Cysharp.Threading.Tasks;

namespace Core.Scripts.Implementation
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

