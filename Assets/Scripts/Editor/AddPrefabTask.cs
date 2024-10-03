namespace Editor
{
    public class AddPrefabTask : Task
    {
        private string moduleName;
        private string targetModuleFolderPath;

        public AddPrefabTask(string moduleName, string targetModuleFolderPath)
        {
            this.moduleName = moduleName;
            this.targetModuleFolderPath = targetModuleFolderPath;
            WaitForCompilation = true;
        }

        public override void Execute() => 
            PrefabCreator.CreatePrefabForModule(moduleName, targetModuleFolderPath);
    }
}