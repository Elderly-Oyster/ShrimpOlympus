namespace Modules.Base.RoguelikeScreen.Scripts.Behaviour.Nodes
{
    public class BehaviourTree : Node
    {
        private readonly bool _autoReset;
        public BehaviourTree(string name = "BehTree", bool autoReset = false, int priority = 0) : base(name, priority)
        {
            _autoReset = autoReset;
        }

        public override Status Process()
        {
            while (CurrentChildIndex < Children.Count)
            {
                var status = Children[CurrentChildIndex].Process();
                if (status != Status.Success)
                {
                    return status;
                }

                CurrentChildIndex++;
            }

            if (_autoReset)
            {
                Reset();
            }
            return Status.Success;
        }
    }
}