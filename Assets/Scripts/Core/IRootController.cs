using Cysharp.Threading.Tasks;

namespace Scripts.Core
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

