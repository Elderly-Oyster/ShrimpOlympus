using Modules.Base.RoguelikeScreen.Scripts.Behaviour.Strategies;

namespace Modules.Base.RoguelikeScreen.Scripts.Behaviour.Nodes
{
    public class ConditionalLeaf : Leaf
    {
        private readonly ICondition _condition;
        
        public ConditionalLeaf(string name, IStrategy action, ICondition condition, int priority = 0) : base(name, action, priority)
        {
            _condition = condition;
        }

        public override Status Process()
        {
            var conditionStatus = _condition.Check();
            if(!conditionStatus)
                return Status.Failure;
            return base.Process();
        }
    }
}