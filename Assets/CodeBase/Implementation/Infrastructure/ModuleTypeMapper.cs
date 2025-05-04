using System;
using System.Collections.Generic;
using CodeBase.Core.Infrastructure;
using CodeBase.Core.Modules;
using CodeBase.Core.Modules.MVP;
using CodeBase.Core.Patterns.Architecture.MVP;
using Modules.Base.ConverterScreen.Scripts;
using Modules.Base.DeliveryTycoon.Scripts;
using Modules.Base.MainMenuScreen.Scripts;
using Modules.Base.StartGameScreen.Scripts;
using Modules.Base.TicTacScreen.Scripts;
using VContainer;

namespace CodeBase.Implementation.Infrastructure
{
    ///<summary> 
    /// Responsible for resolving (or instantiating) the appropriate module controller for the
    /// specified ModulesMap, using a dependency injection container provided by sceneLifetimeScope
    ///</summary>
    public class ModuleTypeMapper
    {
        private readonly Dictionary<ModulesMap, Type> _map;

        public ModuleTypeMapper()
        {
            _map = new Dictionary<ModulesMap, Type> 
            {
                { ModulesMap.StartGame, typeof(StartGameScreenPresenter) },
                { ModulesMap.Converter, typeof(ConverterScreenPresenter) },
                { ModulesMap.MainMenu, typeof(MainMenuScreenPresenter) },
                { ModulesMap.TicTac, typeof(TicTacScreenPresenter) },
                { ModulesMap.DeliveryTycoon, typeof(GameModuleController)}
            };
        }

        public IScreenPresenter Resolve(ModulesMap modulesMap, IObjectResolver objectResolver) => 
            (IScreenPresenter)objectResolver.Resolve(_map[modulesMap]);
        
        public IModuleController ResolveModuleController(ModulesMap modulesMap, IObjectResolver objectResolver) =>
        (IModuleController)objectResolver.Resolve(_map[modulesMap]);
    }
}