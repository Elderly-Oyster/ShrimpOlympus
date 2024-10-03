namespace Editor
{
    public class AddPrefabTask : Task
    {
        private readonly string _moduleName;
        private readonly string _targetModuleFolderPath;

        public AddPrefabTask(string moduleName, string targetModuleFolderPath)
        {
            this._moduleName = moduleName;
            this._targetModuleFolderPath = targetModuleFolderPath;
            WaitForCompilation = true;
        }

        public override void Execute() => 
            PrefabCreator.CreatePrefabForModule(_moduleName, _targetModuleFolderPath);
    }
}