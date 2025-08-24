using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modules.Base.RoguelikeScreen.Scripts.Behaviour.Nodes
{
    public class PrioritySelector : Node
    {
        private List<Node> _sortedChildren;
        
        public PrioritySelector(string name, int priority = 0) :  base(name,  priority) {}

        public override Status Process()
        {
            while (CurrentChildIndex < _sortedChildren.Count)
            {
                switch (_sortedChildren[CurrentChildIndex].Process())
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Failure:
                        CurrentChildIndex++;
                        return Status.Running;
                    case Status.Success:
                        Reset();
                        return Status.Success;
                }
            }
            
            Reset();
            return Status.Failure;
        }

        private void SortChildren()
        {
            _sortedChildren = Children.OrderByDescending(child => child.Priority).ToList();
        }

        public override void Reset()
        {
            base.Reset();
            SortChildren();
        }
        
        public override void PrintDebug(StringBuilder builder)
        {
            builder.Append($"{Name} -> ");
            _sortedChildren[CurrentChildIndex].PrintDebug(builder);
        }
    }
}