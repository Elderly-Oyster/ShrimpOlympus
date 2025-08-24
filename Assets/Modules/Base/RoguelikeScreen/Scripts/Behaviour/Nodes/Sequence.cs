namespace Modules.Base.RoguelikeScreen.Scripts.Behaviour.Nodes
{
    public class Sequence : Node
    {
        public Sequence(string name, int priority = 0) :  base(name, priority) {}

        public override Status Process()
        {
            if (CurrentChildIndex < Children.Count)
            {
                switch (Children[CurrentChildIndex].Process())
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Failure:
                        Reset();
                        return Status.Failure;
                    case Status.Success:
                        CurrentChildIndex++;
                        return CurrentChildIndex == Children.Count ? Status.Success : Status.Running;
                }
            }
            
            Reset();
            return Status.Success;
        }
    }
}