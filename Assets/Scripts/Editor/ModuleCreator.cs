using Editor.Tasks;
using Editor.Tasks.TaskQueue;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class ModuleCreator : EditorWindow
    {
        private FolderType _selectedFolder = FolderType.Base;

        public enum FolderType { Additional, Base, Test }
        private const float GUISpacing = 10f;
        private string _moduleName = "NewModule";
        private bool _createInstaller = true;
        private bool _createPresenter = true;
        private bool _createView = true;
        private bool _createModel = true;
        private bool _createAsmdef = true;

        [MenuItem("Tools/Create Module")]
        public static void ShowWindow() => GetWindow<ModuleCreator>("Create Module");

        private static bool IsValidModuleName(string moduleName) =>
            !string.IsNullOrWhiteSpace(moduleName) && !moduleName.Contains(" ");

        private void OnGUI()
        {
            GUILayout.Label("Module Creator", EditorStyles.boldLabel);

            DrawModuleNameField();
            GUILayout.Space(GUISpacing);

            DrawFolderSelection();
            GUILayout.Space(GUISpacing);

            DrawScriptOptions();
            GUILayout.Space(GUISpacing);

            DrawAsmdefOption();
            GUILayout.Space(GUISpacing);

            HandleCreateModuleButton();
        }

        private void DrawModuleNameField() =>
            _moduleName = EditorGUILayout.TextField("Module Name", _moduleName);

        private void DrawFolderSelection() =>
            _selectedFolder = (FolderType)EditorGUILayout.EnumPopup("Select Folder", _selectedFolder);

        private void DrawScriptOptions()
        {
            GUILayout.Label("Select Scripts to Create", EditorStyles.boldLabel);
            _createInstaller = EditorGUILayout.Toggle("Installer", _createInstaller);
            _createPresenter = EditorGUILayout.Toggle("Presenter", _createPresenter);
            _createView = EditorGUILayout.Toggle("View", _createView);
            _createModel = EditorGUILayout.Toggle("Model", _createModel);
        }

        private void DrawAsmdefOption() =>
            _createAsmdef = EditorGUILayout.Toggle("Create asmdef", _createAsmdef);

        private void HandleCreateModuleButton()
        {
            if (GUILayout.Button("Create Module"))
            {
                if (IsValidModuleName(_moduleName))
                    CreateModule();
                else
                {
                    EditorUtility.DisplayDialog("Invalid Name",
                        "Module name cannot be empty or contain spaces.", "OK");
                }
            }
        }

        private void CreateModule()
        {
            Debug.Log("Creating module with name: " + _moduleName);

            var addScriptsTask = new AddScriptsTask(
                _moduleName,
                _selectedFolder.ToString(),
                _createInstaller,
                _createPresenter,
                _createView,
                _createModel,
                _createAsmdef);

            TaskQueue.EnqueueTask(addScriptsTask);

            var targetModuleFolderPath = ModuleGenerator.GetTargetModuleFolderPath(_moduleName, _selectedFolder.ToString());

            var addPrefabTask = new AddPrefabTask(
                _moduleName,
                targetModuleFolderPath);

            TaskQueue.EnqueueTask(addPrefabTask);

            EditorUtility.DisplayDialog("Module Creation Started",
                "Module creation process has started. Please wait for it to complete.", "OK");

            _moduleName = "NewModule";
        }
    }
}
