using System.Collections.Generic;
using System.Text;

namespace Modules.Base.RoguelikeScreen.Scripts.Behaviour.Nodes
{
    public class Node
    {
        public enum Status
        {
            Success,
            Failure,
            Running
        }

        public readonly string Name;
        
        public readonly int Priority;

        public readonly List<Node> Children = new();
        protected int CurrentChildIndex;

        public Node(string name = "Node", int priority = 0)
        {
            Name = name;
            Priority = priority;
        }

        public void AddChild(Node child) => Children.Add(child);

        public virtual Status Process() => Children[CurrentChildIndex].Process();

        public virtual void Reset()
        {
            CurrentChildIndex = 0;
            foreach (var child in Children)
            {
                child.Reset();
            }
        }

        public virtual void PrintDebug(StringBuilder builder)
        {
            builder.Append($"{Name} -> ");
            Children[CurrentChildIndex].PrintDebug(builder);
        }
    }
}