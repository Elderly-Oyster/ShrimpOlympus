using System.Text;
using Modules.Base.RoguelikeScreen.Scripts.Behaviour.Strategies;

namespace Modules.Base.RoguelikeScreen.Scripts.Behaviour.Nodes
{
    public class Leaf : Node
    {
        private readonly IStrategy _strategy;

        public Leaf(string name, IStrategy strategy, int priority = 0) : base(name, priority)
        {
            _strategy = strategy;
        }

        public override Status Process() =>
            _strategy.Process();

        public override void Reset() =>
            _strategy.Reset();
        
        public override void PrintDebug(StringBuilder builder)
        {
            builder.Append($"{Name}");
        }
    }
}