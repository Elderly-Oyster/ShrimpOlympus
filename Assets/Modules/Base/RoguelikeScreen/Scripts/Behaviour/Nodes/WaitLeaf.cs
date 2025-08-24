using System.Text;
using Modules.Base.RoguelikeScreen.Scripts.Behaviour.Strategies;

namespace Modules.Base.RoguelikeScreen.Scripts.Behaviour.Nodes
{
    public class WaitLeaf : Node
    {
        private float _waitTime;
        private float _currentTimeWaited;
        public WaitLeaf(string name, float waitTime, int priority = 0) : base(name, priority)
        {
            _waitTime = waitTime;
        }

        public override Status Process()
        {
            _currentTimeWaited += UnityEngine.Time.deltaTime;
            if(_currentTimeWaited >= _waitTime)
                return Status.Success;
            return Status.Running;
        }

        public override void Reset()
        {
            _currentTimeWaited = 0;
        }
        
        public override void PrintDebug(StringBuilder builder)
        {
            builder.Append($"{Name}");
        }
    }
}