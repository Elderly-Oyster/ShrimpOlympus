using System;
using Core.Scripts;
using Core.Scripts.MVP;
using Cysharp.Threading.Tasks;
using VContainer;

namespace Implementation.Scripts
{
    public interface  IScreenStateMachine
    {
        public IScreenPresenter CurrentPresenter { get; }

        public event Action<IObjectResolver> ModuleChanged;
        
        UniTaskVoid RunScreen(ScreenPresenterMap screenPresenterMap, object param = null);
    }
    
    public static class RootControllerExtension
    {
        public static UniTaskVoid RunModel(this IScreenStateMachine self, ScreenPresenterMap screenPresenterMap) => 
            self.RunScreen(screenPresenterMap);
    }
}

