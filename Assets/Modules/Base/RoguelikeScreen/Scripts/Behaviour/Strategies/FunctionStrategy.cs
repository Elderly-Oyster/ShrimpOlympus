using System;
using Modules.Base.RoguelikeScreen.Scripts.Behaviour.Nodes;

namespace Modules.Base.RoguelikeScreen.Scripts.Behaviour.Strategies
{
    public class FunctionStrategy
    {
        private readonly Func<Node.Status> _function;

        public FunctionStrategy(Func<Node.Status> function) => _function  = function;
        
        public Node.Status Process() => _function();

        public void Reset() {}
    }
}