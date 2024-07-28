using System;
using System.Collections.Generic;
using Core;
using Modules.ConverterScreen.Scripts;
using Modules.MainMenuScreen.Scripts;
using Modules.StartGameScreen.Scripts;
using Modules.TicTacScreen.Scripts;
using VContainer;

namespace Startup   
{
    public class ScreenTypeMapper
    {
        private readonly Dictionary<ScreenModelMap, Type> _map;

        public ScreenTypeMapper()
        {
            _map = new Dictionary<ScreenModelMap, Type>
            {
                { ScreenModelMap.StartGame, typeof(StartGameScreenModel) },
                { ScreenModelMap.Converter, typeof(ConverterScreenModel) },
                { ScreenModelMap.MainMenu, typeof(MainMenuScreenModel) },
                { ScreenModelMap.TicTac, typeof(TicTacScreenModel) }
            };
        }

        public IScreenModel Resolve(ScreenModelMap screenModelMap, IObjectResolver objectResolver) =>
            (IScreenModel)objectResolver.Resolve(_map[screenModelMap]);
    }
}