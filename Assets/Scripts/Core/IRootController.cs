using Cysharp.Threading.Tasks;

namespace Core
{
    public interface  IRootController
    {
        UniTaskVoid RunPresenter(ScreenPresenterMap screenPresenterMap, object param = null);
    }
    
    public static class RootControllerExtension
    {
        public static UniTaskVoid RunPresenter(this IRootController self, ScreenPresenterMap screenPresenterMap) => 
            self.RunPresenter(screenPresenterMap);
    }
    
    public enum ScreenPresenterMap
    {
        StartGame = 0,
        Converter = 1
    }
}