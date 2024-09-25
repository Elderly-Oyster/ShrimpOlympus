using System;
using System.Collections.Generic;
using Core.Scripts;
using Core.Scripts.MVP;
using Modules.Scripts.Base.ConverterScreen.Scripts;
using Modules.Scripts.Base.MainMenuScreen.Scripts;
using Modules.Scripts.Base.StartGameScreen.Scripts;
using Modules.Scripts.Base.TicTacScreen.Scripts;
using VContainer;

namespace Root.Scripts.ScreenStateMachine   
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