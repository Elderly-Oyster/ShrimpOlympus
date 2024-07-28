using System;
using System.Collections.Generic;
using Scripts.Core;
using Scripts.Modules.ConverterScreen.Scripts;
using Scripts.Modules.MainMenuScreen.Scripts;
using Scripts.Modules.StartGameScreen.Scripts;
using Scripts.Modules.TicTacScreen.Scripts;
using VContainer;

namespace Scripts.Startup   
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