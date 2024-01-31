using Cysharp.Threading.Tasks;

namespace Core
{
    public interface  IRootController
    {
        UniTaskVoid RunPresenter(ScreenPresenterMap screenPresenterMap, object param = null);
    }
    
    public static class RootControllerExtension
    {
        public static UniTaskVoid RunPresenter(this IRootController self, ScreenPresenterMap screenPresenterMap)
        {
            return self.RunPresenter(screenPresenterMap);
        }
    }
    
    public enum ScreenPresenterMap
    {
        StartGame = 0,
        MainMenu = 1,
        PlayMode = 2
    }
}