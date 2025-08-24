using System;
using Modules.Base.RoguelikeScreen.Scripts.Behaviour.Nodes;

namespace Modules.Base.RoguelikeScreen.Scripts.Behaviour.Strategies
{
    public class ActionStrategy : IStrategy
    {
        private readonly Action _action;

        public ActionStrategy(Action action) => _action = action;
        public Node.Status Process()
        {
            _action();
            return Node.Status.Success;
        }

        public void Reset() {}
    }
}