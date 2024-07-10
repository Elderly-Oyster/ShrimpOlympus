using System;
using System.Collections.Generic;
using MVP.MVP_Root_Model.Scripts.Core;
using MVP.MVP_Root_Model.Scripts.Modules.ConverterScreen.Scripts;
using MVP.MVP_Root_Model.Scripts.Modules.MainMenuScreen.Scripts;
using MVP.MVP_Root_Model.Scripts.Modules.StartGameScreen.Scripts;
using MVP.MVP_Root_Model.Scripts.Modules.TicTacScreen.Scripts;
using VContainer;
using VContainer.Unity;

namespace MVP.MVP_Root_Model.Scripts.Startup
{
    public class ScreenTypeMapper
    {
        private readonly Dictionary<ScreenModelMap, Type> _map;
        private readonly IObjectResolver _resolver; //Root Resolver

        public ScreenTypeMapper(IObjectResolver resolver)
        {
            _resolver = resolver;
            _map = new Dictionary<ScreenModelMap, Type>
            {
                { ScreenModelMap.StartGame, typeof(StartGameScreenModel) },
                { ScreenModelMap.Converter, typeof(ConverterScreenModel) },
                { ScreenModelMap.MainMenu, typeof(MainMenuScreenModel) },
                { ScreenModelMap.TicTac, typeof(TicTacScreenModel) }
            };
        }

        public IScreenModel Resolve(ScreenModelMap screenModelMap, LifetimeScope lifetimeScope) =>
            (IScreenModel)lifetimeScope.Container.Resolve(_map[screenModelMap]);
    }
}