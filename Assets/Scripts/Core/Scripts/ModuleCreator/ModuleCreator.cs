using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Core.Scripts.ModuleCreator
{
    public class ModuleCreator : EditorWindow
    {
        private BaseModuleCreator.FolderType _selectedFolder = BaseModuleCreator.FolderType.Base;

        private string _moduleName = "NewModule";
        private bool _createInstaller = true;
        private bool _createPresenter = true;
        private bool _createView = true;
        private bool _createModel = true;
        private bool _createAsmdef = true;
        private bool _createPrefab = true;

        private BaseModuleCreator _baseModuleCreator;
        private ScriptModuleCreator _scriptModuleCreator;
        private AsmdefCreator _asmdefCreator;
        private PrefabModuleCreator _prefabModuleCreator;

        private readonly List<string> _requiredTemplates = new List<string>
        {
            "TemplateScreenInstaller.cs",
            "TemplateScreenPresenter.cs",
            "TemplateScreenView.cs",
            "TemplateScreenModel.cs"
        };

        [MenuItem("Tools/Create Module")]
        public static void ShowWindow() => GetWindow<ModuleCreator>("Create Module");

        private void OnEnable()
        {
            _baseModuleCreator = new BaseModuleCreator();
            _baseModuleCreator.InitializePaths();
            _baseModuleCreator.EnsureSubfoldersExist();

            _scriptModuleCreator = new ScriptModuleCreator(_baseModuleCreator.TemplateFolderPath);
            _asmdefCreator = new AsmdefCreator(_baseModuleCreator.TemplateModuleFolderPath);
            _prefabModuleCreator = new PrefabModuleCreator(_baseModuleCreator.TemplateModuleFolderPath);
        }

        private void OnGUI()
        {
            GUILayout.Label("Module Creator", EditorStyles.boldLabel);
            _moduleName = EditorGUILayout.TextField("Module Name", _moduleName);
            GUILayout.Space(BaseModuleCreator.GUISpacing);
            _selectedFolder = (BaseModuleCreator.FolderType)EditorGUILayout.
                EnumPopup("Select Folder", _selectedFolder);
            GUILayout.Space(BaseModuleCreator.GUISpacing);
            GUILayout.Label("Select Items to Create", EditorStyles.boldLabel);
            _createInstaller = EditorGUILayout.Toggle("Installer", _createInstaller);
            _createPresenter = EditorGUILayout.Toggle("Presenter", _createPresenter);
            _createView = EditorGUILayout.Toggle("View", _createView);
            _createModel = EditorGUILayout.Toggle("Model", _createModel);
            _createPrefab = EditorGUILayout.Toggle("Prefab", _createPrefab);

            GUILayout.Space(BaseModuleCreator.GUISpacing);
            _createAsmdef = EditorGUILayout.Toggle("Create asmdef", _createAsmdef);

            GUILayout.Space(BaseModuleCreator.GUISpacing);
            if (GUILayout.Button("Create Module"))
            {
                if (BaseModuleCreator.IsValidModuleName(_moduleName))
                {
                    if (AreTemplatesAvailable())
                        CreateModuleFiles(_moduleName);
                }
                else
                {
                    EditorUtility.DisplayDialog("Invalid Name",
                        "Module name cannot be empty or contain spaces.", "OK");
                }
            }
        }

        private bool AreTemplatesAvailable()
        {
            if (!AssetDatabase.IsValidFolder(_baseModuleCreator.TemplateFolderPath))
            {
                EditorUtility.DisplayDialog("Missing Template Folder",
                    $"Template folder not found at {_baseModuleCreator.TemplateFolderPath}.\n" +
                    $"\nModule creation aborted.",
                    "OK");
                return false;
            }

            var missingTemplates = _requiredTemplates.Where(template =>
                !File.Exists(Path.Combine(_baseModuleCreator.TemplateFolderPath, template))).ToList();

            if (missingTemplates.Any())
            {
                string missing = string.Join("\n", missingTemplates);
                EditorUtility.DisplayDialog("Missing Templates",
                    $"The following template files are missing:\n{missing}\n\nModule creation aborted.",
                    "OK");
                return false;
            }

            if (_createAsmdef)
            {
                string templateAsmdefPath = Path.Combine(_baseModuleCreator.TemplateModuleFolderPath, "TemplateScreen.asmdef");
                if (!File.Exists(templateAsmdefPath))
                {
                    EditorUtility.DisplayDialog("Missing asmdef Template",
                        $"Template asmdef file not found at {templateAsmdefPath}.\n\nModule creation aborted.",
                        "OK");
                    return false;
                }
            }

            if (_createPrefab)
            {
                string templatePrefabPath = Path.Combine(_baseModuleCreator.TemplateModuleFolderPath, "Views", "TemplateView.prefab");
                if (!File.Exists(templatePrefabPath))
                {
                    EditorUtility.DisplayDialog("Missing Prefab Template",
                        $"Template prefab not found at {templatePrefabPath}.\n\nModule creation aborted.",
                        "OK");
                    return false;
                }
            }

            return true;
        }

        private void CreateModuleFiles(string moduleName)
        {
            string selectedFolderPath = _baseModuleCreator.GetSelectedFolderPath(_selectedFolder);
            string targetFolderPath = Path.Combine(selectedFolderPath, $"{moduleName}Screen");
            BaseModuleCreator.EnsureTargetFolderExists(targetFolderPath);

            string scriptsFolderPath = Path.Combine(targetFolderPath, "Scripts");
            BaseModuleCreator.EnsureTargetFolderExists(scriptsFolderPath);

            if (_createAsmdef)
                _asmdefCreator.CreateAsmdefFile(targetFolderPath, moduleName);

            _scriptModuleCreator.CreateSelectedScripts(scriptsFolderPath, moduleName,
                _selectedFolder, _createInstaller, _createPresenter, _createView, _createModel);

            AssetDatabase.Refresh();

            EditorApplication.delayCall += () =>
            {
                if (_createPrefab) _prefabModuleCreator.CreatePrefab(targetFolderPath, moduleName);

                AssetDatabase.Refresh();

                EditorUtility.DisplayDialog("Success",
                    $"Module {moduleName} created successfully.", "OK");
            };
        }
    }
}
