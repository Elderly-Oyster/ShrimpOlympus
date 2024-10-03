using System;
using Editor.Tasks.Abstract;
using UnityEditor;

namespace Editor.Tasks
{
    [Serializable]
    public class AddScriptsTask : Task
    {
        private string _moduleName;
        private string _selectedFolder;
        private bool _createInstaller;
        private bool _createPresenter;
        private bool _createView;
        private bool _createModel;
        private bool _createAsmdef;

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