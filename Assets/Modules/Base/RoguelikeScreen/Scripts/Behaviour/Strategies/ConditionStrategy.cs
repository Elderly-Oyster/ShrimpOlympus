using System;
using Modules.Base.RoguelikeScreen.Scripts.Behaviour.Nodes;

namespace Modules.Base.RoguelikeScreen.Scripts.Behaviour.Strategies
{
    public class ConditionStrategy : IStrategy
    {
        private readonly Func<bool> _condition;

        public ConditionStrategy(Func<bool> condition)
        {
            _condition = condition;
        }
        
        public Node.Status Process() => _condition() ? Node.Status.Success : Node.Status.Failure;

        public void Reset() {}
    }
}