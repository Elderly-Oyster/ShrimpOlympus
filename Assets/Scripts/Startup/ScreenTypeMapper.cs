using System;
using System.Collections.Generic;
using Core;
using Core.MVVM;
using Modules.Base.ConverterScreen.Scripts;
using Modules.Base.MainMenuScreen.Scripts;
using Modules.Base.StartGameScreen.Scripts;
using Modules.Base.TicTacScreen.Scripts;
using VContainer;

namespace Startup   
{
    public class ScreenTypeMapper
    {
        private readonly Dictionary<ScreenPresenterMap, Type> _map;

        public ScreenTypeMapper()
        {
            _map = new Dictionary<ScreenPresenterMap, Type> //TODO Заменить модели на презентеры
            {
                { ScreenPresenterMap.StartGame, typeof(StartGameScreenModel) },
                { ScreenPresenterMap.Converter, typeof(ConverterScreenModel) },
                { ScreenPresenterMap.MainMenu, typeof(MainMenuScreenModel) },
                { ScreenPresenterMap.TicTac, typeof(TicTacScreenModel) }
            };
        }

        public IScreenModel Resolve(ScreenPresenterMap screenPresenterMap, IObjectResolver objectResolver) =>
            (IScreenModel)objectResolver.Resolve(_map[screenPresenterMap]);
    }
}