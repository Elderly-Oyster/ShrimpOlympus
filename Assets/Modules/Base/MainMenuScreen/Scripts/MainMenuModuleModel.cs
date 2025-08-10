using CodeBase.Core.Patterns.Architecture.MVP;

namespace Modules.Base.MainMenuScreen.Scripts
{
    public class MainMenuModuleModel : IModel
    {
        // Throttle delays for anti-spam protection
        public int CommandThrottleDelay { get; } = 300;
        public int ModuleTransitionThrottleDelay { get; } = 500;
        
        public MainMenuModuleModel() { }

        public void Dispose() { }
    }
}