using CodeBase.Core.Patterns.Architecture.MVP;
using Cysharp.Threading.Tasks;

namespace CodeBase.Core.Infrastructure
{
    public interface  IScreenStateMachine
    {
        public IPresenter CurrentPresenter { get; }
        
        UniTaskVoid RunModule(ModulesMap modulesMap, object param = null);
    }
    
    public static class RootControllerExtension
    {
        public static UniTaskVoid RunModel(this IScreenStateMachine self, ModulesMap modulesMap) => 
            self.RunModule(modulesMap);
    }
}

