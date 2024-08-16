using System;
using Cysharp.Threading.Tasks;
using VContainer;

namespace Core
{
    //TODO Переделать систему под рутовую ViewModel
    public interface  IScreenStateMachine
    {
        UniTaskVoid RunViewModel(ScreenPresenterMap screenPresenterMap, object param = null);
        
        public event Action<IObjectResolver> ModuleChanged;
    }
    
    public static class RootControllerExtension
    {
        public static UniTaskVoid RunModel(this IScreenStateMachine self, ScreenPresenterMap screenPresenterMap) => 
            self.RunViewModel(screenPresenterMap);
    }
    
    public enum ScreenPresenterMap
    {
        StartGame,
        MainMenu,
        Converter,
        TicTac,
    }
}

