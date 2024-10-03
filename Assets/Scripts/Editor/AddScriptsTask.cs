using UnityEditor;

namespace Editor
{
    public class AddScriptsTask : Task
    {
        private string moduleName;
        private string selectedFolder;
        private bool createInstaller;
        private bool createPresenter;
        private bool createView;
        private bool createModel;
        private bool createAsmdef;

        public AddScriptsTask(string moduleName, string selectedFolder, bool createInstaller, bool createPresenter, bool createView, bool createModel, bool createAsmdef)
        {
            this.moduleName = moduleName;
            this.selectedFolder = selectedFolder;
            this.createInstaller = createInstaller;
            this.createPresenter = createPresenter;
            this.createView = createView;
            this.createModel = createModel;
            this.createAsmdef = createAsmdef;
            this.WaitForCompilation = true;
        }

        public override void Execute()
        {
            PathManager.InitializePaths();

            if (TemplateValidator.AreTemplatesAvailable(createAsmdef))
            {
                ModuleGenerator.CreateModuleFiles(
                    moduleName,
                    selectedFolder,
                    createInstaller,
                    createPresenter,
                    createView,
                    createModel,
                    createAsmdef);

                AssetDatabase.Refresh();
            }
        }
    }
}