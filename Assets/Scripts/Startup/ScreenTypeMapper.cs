using System;
using System.Collections.Generic;
using Core;
using Modules.MainMenu.Scripts;
using Modules.StartGame.Scripts;
using VContainer;

namespace Startup
{
    public class ScreenTypeMapper
    {
        private readonly Dictionary<ScreenPresenterMap, Type> _map;
        private readonly IObjectResolver _resolver;

        public ScreenTypeMapper(IObjectResolver resolver)
        {
            _resolver = resolver;
            _map = new Dictionary<ScreenPresenterMap, Type>
            {
                { ScreenPresenterMap.StartGame, typeof(StartGameScreenPresenter) },
                { ScreenPresenterMap.MainMenu, typeof(ConverterPresenter) },
            };
        }

        public IScreenPresenter Resolve(ScreenPresenterMap screenPresenterMap) => 
            (IScreenPresenter)_resolver.Resolve(_map[screenPresenterMap]);
    }
}