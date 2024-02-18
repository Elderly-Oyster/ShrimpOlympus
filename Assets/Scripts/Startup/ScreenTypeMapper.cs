using System;
using System.Collections.Generic;
using Core;
using Modules.ConverterScreen.Scripts;
using Modules.StartGame.Scripts;
using VContainer;

namespace Startup
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