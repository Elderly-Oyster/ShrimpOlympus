using Modules.Base.RoguelikeScreen.Scripts.Behaviour.Nodes;

namespace Modules.Base.RoguelikeScreen.Scripts.Behaviour.Strategies
{
    public interface IStrategy
    {
        public Node.Status Process();
        public void Reset();
    }
}