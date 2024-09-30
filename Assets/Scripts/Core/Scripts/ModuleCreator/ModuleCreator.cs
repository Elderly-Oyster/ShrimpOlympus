using UnityEditor;
using UnityEngine;

namespace Core.Scripts.ModuleCreator
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

            if (GUILayout.Button("Create Module"))
            {
                if (IsValidModuleName(_moduleName))
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

                        EditorPrefs.SetBool(ModuleGenerator.ModuleCreationInProgressKey, true);
                        EditorPrefs.SetString(ModuleGenerator.ModuleNameKey, _moduleName);
                        EditorPrefs.SetString(ModuleGenerator.TargetModuleFolderPathKey, ModuleGenerator.TargetModuleFolderPath);

                        AssetDatabase.Refresh();
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("Invalid Name",
                        "Module name cannot be empty or contain spaces.", "OK");
                }
            }
        }
    }
}
