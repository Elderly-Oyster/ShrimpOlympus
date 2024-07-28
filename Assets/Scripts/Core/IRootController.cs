using Cysharp.Threading.Tasks;

namespace Core
{
    public interface  IScreenController
    {
        UniTaskVoid RunModel(ScreenModelMap screenModelMap, object param = null);
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

