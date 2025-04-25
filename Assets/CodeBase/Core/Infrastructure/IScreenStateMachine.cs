using CodeBase.Core.Modules;
using Cysharp.Threading.Tasks;

namespace CodeBase.Core.Infrastructure
{
    public interface  IScreenStateMachine
    {
        public IScreenPresenter CurrentPresenter { get; }
        
        UniTaskVoid RunScreen(ModulesMap modulesMap, object param = null);
    }
    
    public static class RootControllerExtension
    {
        public static UniTaskVoid RunModel(this IScreenStateMachine self, ModulesMap modulesMap) => 
            self.RunScreen(modulesMap);
    }
}

