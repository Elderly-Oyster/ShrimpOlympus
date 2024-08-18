using System;
using Cysharp.Threading.Tasks;
using VContainer;

namespace Core
{
    //TODO Переделать систему под рутовую ViewModel
    public interface  IScreenStateMachine
    {
        UniTaskVoid RunPresenter(ScreenPresenterMap screenPresenterMap, object param = null);
        
        public event Action<IObjectResolver> ModuleChanged;
    }
    
    public static class RootControllerExtension
    {
        public static UniTaskVoid RunModel(this IScreenStateMachine self, ScreenPresenterMap screenPresenterMap) => 
            self.RunPresenter(screenPresenterMap);
    }
    
    public enum ScreenPresenterMap
    {
        StartGame,
        MainMenu,
        Converter,
        TicTac,
    }
}

