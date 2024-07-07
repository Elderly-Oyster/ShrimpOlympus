using System;
using System.Collections.Generic;
using MVP.MVP_Root_Model.Scripts.Core;
using MVP.MVP_Root_Model.Scripts.Modules.ConverterScreen.Scripts;
using MVP.MVP_Root_Model.Scripts.Modules.StartGame.Scripts;
using UnityEngine;
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

            Debug.Log("ScreenTypeMapper initialized with mappings");
        }

        public IScreenModel Resolve(ScreenModelMap screenModelMap)
        {
            Debug.Log($"Resolving {screenModelMap}");

            if (_map.TryGetValue(screenModelMap, out var type))
            {
                Debug.Log($"Found type: {type}");
                var resolvedModel = _resolver.Resolve(type) as IScreenModel;

                if (resolvedModel == null)
                {
                    Debug.LogError($"Failed to resolve model for type {type}");
                }

                return resolvedModel;
            }
            
            Debug.LogError($"Type for {screenModelMap} not found in map");
            return null;
        }
    }
}