namespace Modules.Base.RoguelikeScreen.Scripts.Behaviour.Nodes
{
    public class Selector : Node
    {
        public Selector(string name, int priority = 0) :  base(name,  priority) {}

        public override Status Process()
        {
            while (CurrentChildIndex < Children.Count)
            {
                switch (Children[CurrentChildIndex].Process())
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Failure:
                        CurrentChildIndex++;
                        return Status.Running;
                    case Status.Success:
                        return Status.Success;
                }
            }
            
            Reset();
            return Status.Failure;
        }
    }
}