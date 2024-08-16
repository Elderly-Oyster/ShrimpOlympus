using System;
using Cysharp.Threading.Tasks;
using VContainer;

namespace Core
{
    //TODO Переделать систему под рутовую ViewModel
    public interface  IScreenStateMachine
    {
        UniTaskVoid RunViewModel(ScreenModelMap screenModelMap, object param = null);
        
        public event Action<IObjectResolver> ModuleChanged;
    }
    
    public static class RootControllerExtension
    {
        public static UniTaskVoid RunModel(this IScreenStateMachine self, ScreenModelMap screenModelMap) => 
            self.RunViewModel(screenModelMap);
    }
    
    public enum ScreenModelMap
    {
        StartGame,
        MainMenu,
        Converter,
        TicTac,
    }
}

