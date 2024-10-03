using UnityEditor;

namespace Editor
{
    public class AddScriptsTask : Task
    {
        private readonly string _moduleName;
        private readonly string _selectedFolder;
        private readonly bool _createInstaller;
        private readonly bool _createPresenter;
        private readonly bool _createView;
        private readonly bool _createModel;
        private readonly bool _createAsmdef;

        public AddScriptsTask(string moduleName, string selectedFolder, bool createInstaller,
            bool createPresenter, bool createView, bool createModel, bool createAsmdef)
        {
            _moduleName = moduleName;
            _selectedFolder = selectedFolder;
            _createInstaller = createInstaller;
            _createPresenter = createPresenter;
            _createView = createView;
            _createModel = createModel;
            _createAsmdef = createAsmdef;
            WaitForCompilation = true;
        }

        public override void Execute()
        {
            PathManager.InitializePaths();

            if (TemplateValidator.AreTemplatesAvailable(_createAsmdef))
            {
                ModuleGenerator.CreateModuleFiles(
                    _moduleName,
                    _selectedFolder,
                    _createInstaller,
                    _createPresenter,
                    _createView,
                    _createModel,
                    _createAsmdef);

                AssetDatabase.Refresh();
            }
        }
    }
}