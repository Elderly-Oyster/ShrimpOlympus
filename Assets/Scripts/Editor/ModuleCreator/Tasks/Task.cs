using System;

namespace Editor.ModuleCreator.Tasks
{
    [Serializable]
    public abstract class Task
    {
        public bool WaitForCompilation { get; protected set; }
        public abstract void Execute();
    }
}