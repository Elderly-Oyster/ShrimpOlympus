using System;
using Cysharp.Threading.Tasks;
using VContainer;

namespace Core
{
    public interface  IScreenController
    {
        UniTaskVoid RunModel(ScreenModelMap screenModelMap, object param = null);
        
        public event Action<IObjectResolver> ModuleChanged;
    }
    
    public static class RootControllerExtension
    {
        public static UniTaskVoid RunModel(this IScreenController self, ScreenModelMap screenModelMap) => 
            self.RunModel(screenModelMap);
    }
    
    public enum ScreenModelMap
    {
        StartGame,
        MainMenu,
        Converter,
        TicTac,
    }
}

