using System;
using System.Collections.Generic;
using CodeBase.Core.Infrastructure;
using CodeBase.Core.Modules;
using CodeBase.Core.Patterns.Architecture.MVP;
using Modules.Base.ConverterScreen.Scripts;
using Modules.Base.DeliveryTycoon.Scripts;
using Modules.Base.MainMenuScreen.Scripts;
using Modules.Base.StartGameScreen.Scripts;
using Modules.Base.TicTacScreen.Scripts;
using VContainer;

namespace CodeBase.Implementation.Infrastructure
{
    ///<summary> 
    /// Responsible for resolving (or instantiating) the appropriate screen presenter for the
    /// specified screenPresenterMap, using a dependency injection container provided by sceneLifetimeScope
    ///</summary>
    public class ScreenTypeMapper
    {
        private readonly Dictionary<ScreenPresenterMap, Type> _map;

        public ScreenTypeMapper()
        {
            _map = new Dictionary<ScreenPresenterMap, Type> 
            {
                { ScreenPresenterMap.StartGame, typeof(StartGameScreenPresenter) },
                { ScreenPresenterMap.Converter, typeof(ConverterScreenPresenter) },
                { ScreenPresenterMap.MainMenu, typeof(MainMenuScreenPresenter) },
                { ScreenPresenterMap.TicTac, typeof(TicTacScreenPresenter) },
                { ScreenPresenterMap.DeliveryTycoon, typeof(GameModuleController)}
            };
        }

        public IScreenPresenter Resolve(ScreenPresenterMap screenPresenterMap, IObjectResolver objectResolver) => 
            (IScreenPresenter)objectResolver.Resolve(_map[screenPresenterMap]);
        
        public IModuleController ResolveController(ScreenPresenterMap screenPresenterMap, IObjectResolver objectResolver) =>
        (IModuleController)objectResolver.Resolve(_map[screenPresenterMap]);
    }
}