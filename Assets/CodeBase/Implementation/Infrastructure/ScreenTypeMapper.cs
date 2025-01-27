using System;
using System.Collections.Generic;
using CodeBase.Core.Infrastructure;
using CodeBase.Core.Modules;
using Modules.Base.ConverterScreen.Scripts;
using Modules.Base.MainMenuScreen.Scripts;
using Modules.Base.StartGameScreen.Scripts;
using Modules.Base.TicTacScreen.Scripts;
using VContainer;

namespace CodeBase.Implementation.Infrastructure
{
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
                { ScreenPresenterMap.TicTac, typeof(TicTacScreenPresenter) }
            };
        }

        public IScreenPresenter Resolve(ScreenPresenterMap screenPresenterMap, IObjectResolver objectResolver) => 
            (IScreenPresenter)objectResolver.Resolve(_map[screenPresenterMap]);
    }
}