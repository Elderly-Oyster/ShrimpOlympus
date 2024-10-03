using System;

namespace Editor
{
    [Serializable]
    public class AddPrefabTask : Task
    {
        private string _moduleName;
        private string _targetModuleFolderPath;

        public AddPrefabTask(string moduleName, string targetModuleFolderPath)
        {
            _moduleName = moduleName;
            _targetModuleFolderPath = targetModuleFolderPath;
            WaitForCompilation = true;
        }

        public override void Execute() =>
            PrefabCreator.CreatePrefabForModule(_moduleName, _targetModuleFolderPath);
    }
}