using Editor.ModuleCreator.Tasks;
using Editor.ModuleCreator.Tasks.TaskQueue;
using UnityEditor;
using UnityEngine;

namespace Editor.ModuleCreator
{
    public class ModuleCreator : EditorWindow
    {
        private FolderType _selectedFolder = FolderType.Base;
        private bool _createPrefab = true;

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
            _moduleName = EditorGUILayout.TextField("Module Name", _moduleName);
            GUILayout.Space(GUISpacing);
            _selectedFolder = (FolderType)EditorGUILayout.EnumPopup("Select Folder", _selectedFolder);
            GUILayout.Space(GUISpacing);
            GUILayout.Label("Select Scripts to Create", EditorStyles.boldLabel);
            _createInstaller = EditorGUILayout.Toggle("Installer", _createInstaller);
            _createPresenter = EditorGUILayout.Toggle("Presenter", _createPresenter);
            _createView = EditorGUILayout.Toggle("View", _createView);
            _createModel = EditorGUILayout.Toggle("Model", _createModel);
            GUILayout.Space(GUISpacing);
            _createAsmdef = EditorGUILayout.Toggle("Create asmdef", _createAsmdef);
            GUILayout.Space(GUISpacing);
            _createPrefab = EditorGUILayout.Toggle("Create Prefab", _createPrefab);
            GUILayout.Space(GUISpacing);
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

            var targetModuleFolderPath = ModuleGenerator.
                GetTargetModuleFolderPath(_moduleName, _selectedFolder.ToString());

            if (_createPrefab)
            {
                var addPrefabTask = new AddPrefabTask(
                    _moduleName,
                    targetModuleFolderPath);

                TaskQueue.EnqueueTask(addPrefabTask);
            }

            EditorUtility.DisplayDialog("Module Creation Started",
                "Module creation process has started. Please wait for it to complete.", "OK");

            _moduleName = "NewModule";
        }
    }
}