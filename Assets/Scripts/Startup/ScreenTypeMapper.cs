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