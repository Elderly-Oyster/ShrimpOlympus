using System;
using Core.MVVM;
using Cysharp.Threading.Tasks;
using VContainer;

namespace Core
{
    //TODO Переделать систему под рутовую ViewModel
    public interface  IScreenStateMachine
    {
        public IScreenPresenter CurrentPresenter { get; }

        public event Action<IObjectResolver> ModuleChanged;
        
        UniTaskVoid RunPresenter(ScreenPresenterMap screenPresenterMap, object param = null);
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

