using Cysharp.Threading.Tasks;

namespace Core
{
    public interface  IRootController
    {
        UniTaskVoid RunModel(ScreenModelMap screenModelMap, object param = null);
    }
    
    public static class RootControllerExtension
    {
        public static UniTaskVoid RunModel(this IRootController self, ScreenModelMap screenModelMap) => 
            self.RunModel(screenModelMap);
    }
    
    public enum ScreenModelMap
    {
        StartGame = 0,
        Converter = 1
    }
}