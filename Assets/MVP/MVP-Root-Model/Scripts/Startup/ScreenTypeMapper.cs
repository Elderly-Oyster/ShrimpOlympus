using System;
using System.Collections.Generic;
using MVP.MVP_Root_Model.Scripts.Core;
using MVP.MVP_Root_Model.Scripts.Modules.ConverterScreen.Scripts;
using MVP.MVP_Root_Model.Scripts.Modules.StartGame.Scripts;
using VContainer;

namespace MVP.MVP_Root_Model.Scripts.Startup
{
    public class ScreenTypeMapper
    {
        private readonly Dictionary<ScreenModelMap, Type> _map;
        private readonly IObjectResolver _resolver;

        public ScreenTypeMapper(IObjectResolver resolver)
        {
            _resolver = resolver;
            _map = new Dictionary<ScreenModelMap, Type>
            {
                { ScreenModelMap.StartGame, typeof(StartGameScreenModel) },
                { ScreenModelMap.Converter, typeof(ConverterScreenModel) },
            };
        }

        public IScreenModel Resolve(ScreenModelMap screenModelMap) => 
            (IScreenModel)_resolver.Resolve(_map[screenModelMap]);
    }
}