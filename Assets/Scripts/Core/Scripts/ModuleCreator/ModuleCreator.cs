using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Core.Scripts.ModuleCreator
{
    public class ModuleCreator : EditorWindow
    {
        private FolderType _selectedFolder = FolderType.Base;
        private enum FolderType { Additional, Base, Test }
        private const string BasePath = "Assets/Scripts/Modules";
        private const float GUISpacing = 10f;
        private string _moduleName = "NewModule";
        private bool _createInstaller = true;
        private bool _createPresenter = true;
        private bool _createView = true;
        private bool _createModel = true;
        private bool _createAsmdef = true;
        private string _additionalFolderPath;
        private string _baseFolderPath;
        private string _testFolderPath;
        private string _templateFolderPath;
        private string _templateModuleFolderPath;
        private string _templateViewsFolderPath;
        private string _templateViewPrefabPath;
        private string _targetModuleFolderPath;
        private readonly List<string> _requiredTemplates = new List<string>
        {
            "TemplateScreenInstaller.cs",
            "TemplateScreenPresenter.cs",
            "TemplateScreenView.cs",
            "TemplateScreenModel.cs"
        };
        private const string ModuleCreationInProgressKey = "ModuleCreationInProgress";
        private const string ModuleNameKey = "ModuleCreationName";
        private const string TargetModuleFolderPathKey = "TargetModuleFolderPath";

        [MenuItem("Tools/Create Module")]
        public static void ShowWindow() => GetWindow<ModuleCreator>("Create Module");

        private static bool IsValidModuleName(string moduleName) =>
            !string.IsNullOrWhiteSpace(moduleName) && !moduleName.Contains(" ");

        private void OnEnable()
        {
            InitializePaths();
            EnsureSubfoldersExist();
        }

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
                    if (AreTemplatesAvailable())
                    {
                        CreateModuleFiles(_moduleName);

                        EditorPrefs.SetBool(ModuleCreationInProgressKey, true);
                        EditorPrefs.SetString(ModuleNameKey, _moduleName);
                        EditorPrefs.SetString(TargetModuleFolderPathKey, _targetModuleFolderPath);

                        AssetDatabase.Refresh();
                    }
                }
                else
                    EditorUtility.DisplayDialog("Invalid Name", "Module name cannot be empty or contain spaces.", "OK");
            }
        }

        private void InitializePaths()
        {
            _additionalFolderPath = CombinePaths(BasePath, "Additional");
            _baseFolderPath = CombinePaths(BasePath, "Base");
            _testFolderPath = CombinePaths(BasePath, "Test");
            _templateFolderPath = CombinePaths(BasePath, "Template", "TemplateScreen", "Scripts");
            _templateModuleFolderPath = CombinePaths(BasePath, "Template", "TemplateScreen");
            _templateViewsFolderPath = CombinePaths(BasePath, "Template", "TemplateScreen", "Views");
            _templateViewPrefabPath = CombinePaths(_templateViewsFolderPath, "TemplateView.prefab");
        }

        private static void EnsureSubfoldersExist()
        {
            CreateFolderIfNotExists(CombinePaths(BasePath, "Additional"));
            CreateFolderIfNotExists(CombinePaths(BasePath, "Base"));
            CreateFolderIfNotExists(CombinePaths(BasePath, "Test"));
        }

        private static void CreateFolderIfNotExists(string folderPath)
        {
            folderPath = folderPath.Replace("\\", "/");
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                string parentFolder = Path.GetDirectoryName(folderPath).Replace("\\", "/");
                string newFolderName = Path.GetFileName(folderPath);
                AssetDatabase.CreateFolder(parentFolder, newFolderName);
            }
        }

        private bool AreTemplatesAvailable()
        {
            if (!AssetDatabase.IsValidFolder(_templateFolderPath))
            {
                ShowDialog("Missing Template Folder", $"Template folder not found at {_templateFolderPath}.\n\nModule creation aborted.");
                return false;
            }
            if (MissingTemplateFiles())
                return false;
            if (_createAsmdef && !AsmdefTemplateExists())
                return false;
            if (!PrefabTemplateExists())
                return false;
            return true;
        }

        private bool MissingTemplateFiles()
        {
            var missingTemplates = _requiredTemplates.Where(template =>
                !File.Exists(Path.Combine(_templateFolderPath, template))).ToList();
            if (missingTemplates.Any())
            {
                string missing = string.Join("\n", missingTemplates);
                ShowDialog("Missing Templates", $"The following template files are missing:\n{missing}\n\nModule creation aborted.");
                return true;
            }
            return false;
        }

        private bool AsmdefTemplateExists()
        {
            string templateAsmdefPath = CombinePaths(_templateModuleFolderPath, "TemplateScreen.asmdef");
            if (!File.Exists(templateAsmdefPath))
            {
                ShowDialog("Missing asmdef Template", $"Template asmdef file not found at {templateAsmdefPath}.\n\nModule creation aborted.");
                return false;
            }
            return true;
        }

        private bool PrefabTemplateExists()
        {
            if (!File.Exists(_templateViewPrefabPath))
            {
                ShowDialog("Missing Prefab Template", $"Template prefab not found at {_templateViewPrefabPath}.\n\nModule creation aborted.");
                return false;
            }
            return true;
        }

        private void ShowDialog(string title, string message) => EditorUtility.DisplayDialog(title, message, "OK");

        private string GetSelectedFolderPath() => _selectedFolder switch
        {
            FolderType.Additional => _additionalFolderPath,
            FolderType.Base => _baseFolderPath,
            FolderType.Test => _testFolderPath,
            _ => _baseFolderPath
        };

        private void CreateModuleFiles(string moduleName)
        {
            string selectedFolderPath = GetSelectedFolderPath();
            string targetFolderPath = CombinePaths(selectedFolderPath, $"{moduleName}Screen");
            _targetModuleFolderPath = targetFolderPath;
            EnsureModuleFolders(targetFolderPath);
            string scriptsFolderPath = CombinePaths(targetFolderPath, "Scripts");

            if (_createAsmdef)
                CreateAsmdefFile(targetFolderPath, moduleName);

            CreateSelectedScripts(scriptsFolderPath, moduleName);

            DisplaySuccessMessage(moduleName);
        }

        private void EnsureModuleFolders(string targetFolderPath)
        {
            EnsureTargetFolderExists(targetFolderPath);
            EnsureTargetFolderExists(CombinePaths(targetFolderPath, "Scripts"));
            EnsureTargetFolderExists(CombinePaths(targetFolderPath, "Views"));
        }

        private void CreateAsmdefFile(string targetFolderPath, string moduleName)
        {
            string templateAsmdefPath = CombinePaths(_templateModuleFolderPath, "TemplateScreen.asmdef");
            string targetAsmdefPath = CombinePaths(targetFolderPath, $"{moduleName}Screen.asmdef");
            CopyAndAdjustAsmdef(templateAsmdefPath, targetAsmdefPath, moduleName);
        }

        private static void DisplaySuccessMessage(string moduleName) =>
            EditorUtility.DisplayDialog("Success", $"Module {moduleName} created successfully.", "OK");

        private static void EnsureTargetFolderExists(string targetFolderPath)
        {
            targetFolderPath = targetFolderPath.Replace("\\", "/");
            if (!AssetDatabase.IsValidFolder(targetFolderPath))
            {
                string parentFolder = Path.GetDirectoryName(targetFolderPath).Replace("\\", "/");
                string newFolderName = Path.GetFileName(targetFolderPath);
                AssetDatabase.CreateFolder(parentFolder, newFolderName);
            }
        }

        private void CreateSelectedScripts(string folderPath, string moduleName)
        {
            var scriptsToCreate = new List<(bool shouldCreate, string templateFile, string outputFile)>
            {
                (_createInstaller, "TemplateScreenInstaller.cs", $"{moduleName}ScreenInstaller.cs"),
                (_createPresenter, "TemplateScreenPresenter.cs", $"{moduleName}ScreenPresenter.cs"),
                (_createView, "TemplateScreenView.cs", $"{moduleName}ScreenView.cs"),
                (_createModel, "TemplateScreenModel.cs", $"{moduleName}ScreenModel.cs"),
            };

            foreach (var (shouldCreate, templateFile, outputFile) in scriptsToCreate)
            {
                if (shouldCreate)
                {
                    string content = GetTemplateContent(templateFile, moduleName);
                    CreateScript(folderPath, outputFile, content);
                }
            }
        }

        private string GetTemplateContent(string templateFileName, string moduleName)
        {
            string templateFilePath = CombinePaths(_templateFolderPath, templateFileName);
            string content = ReadTemplateFile(templateFilePath);
            if (content == null)
                return null;

            string moduleNameLower = char.ToLower(moduleName[0]) + moduleName.Substring(1);
            content = ReplaceNamespace(content, moduleName);
            content = ReplaceTemplateOccurrences(content, moduleName, moduleNameLower);
            return content;
        }

        private static string ReadTemplateFile(string templateFilePath) =>
            File.Exists(templateFilePath) ? File.ReadAllText(templateFilePath) : null;

        private string ReplaceNamespace(string content, string moduleName)
        {
            string namespaceReplacement = $"namespace Modules.{_selectedFolder}.{moduleName}Screen.Scripts";
            return Regex.Replace(content, @"namespace\s+[\w\.]+", namespaceReplacement);
        }

        private static string ReplaceTemplateOccurrences(string content, string moduleName, string moduleNameLower) =>
            Regex.Replace(content, @"(_?)(template)", match =>
            {
                string prefix = match.Groups[1].Value;
                string templateWord = match.Groups[2].Value;
                return prefix + (char.IsUpper(templateWord[0]) ? moduleName : moduleNameLower);
            }, RegexOptions.IgnoreCase);

        private static void CreateScript(string folderPath, string fileName, string scriptContent)
        {
            if (string.IsNullOrEmpty(scriptContent))
                return;

            string filePath = CombinePaths(folderPath, fileName);

            if (File.Exists(filePath))
            {
                if (!EditorUtility.DisplayDialog("File Exists", $"File {fileName} already exists. Overwrite?",
                        "Yes", "No"))
                    return;
            }

            WriteToFile(filePath, scriptContent);
        }

        private static void WriteToFile(string filePath, string content)
        {
            try
            {
                File.WriteAllText(filePath, content);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error writing file {Path.GetFileName(filePath)}: {ex.Message}");
            }
        }

        private static void CopyAndAdjustAsmdef(string templateAsmdefPath, string targetAsmdefPath, string moduleName)
        {
            string content = ReadTemplateFile(templateAsmdefPath);
            if (content == null)
            {
                EditorUtility.DisplayDialog("Missing asmdef Template", 
                    $"Template asmdef file not found at {templateAsmdefPath}.\n\nCannot create asmdef file.", "OK");
                return;
            }
            content = AdjustAsmdefContent(content, moduleName);
            WriteToFile(targetAsmdefPath, content);
        }

        private static string AdjustAsmdefContent(string content, string moduleName) =>
            Regex.Replace(content, @"""name"":\s*""[^""]+""", $@"""name"": ""{moduleName}Screen""");

        [InitializeOnLoadMethod]
        private static void OnEditorReload()
        {
            bool moduleCreationInProgress = EditorPrefs.GetBool(ModuleCreationInProgressKey, false);
            if (moduleCreationInProgress)
            {
                string moduleName = EditorPrefs.GetString(ModuleNameKey, "");
                string targetModuleFolderPath = EditorPrefs.GetString(TargetModuleFolderPathKey, "");

                if (!string.IsNullOrEmpty(moduleName) && !string.IsNullOrEmpty(targetModuleFolderPath))
                {
                    EditorApplication.delayCall += () =>
                    {
                        CreatePrefabForModule(moduleName, targetModuleFolderPath);

                        EditorPrefs.DeleteKey(ModuleCreationInProgressKey);
                        EditorPrefs.DeleteKey(ModuleNameKey);
                        EditorPrefs.DeleteKey(TargetModuleFolderPathKey);

                        EditorUtility.DisplayDialog("Module Creation Finished", 
                            "Scripts compiled and prefab created successfully.", "OK");
                    };
                }
            }
        }

        private static void CreatePrefabForModule(string moduleName, string targetModuleFolderPath)
        {
            string targetPrefabFolderPath = CombinePaths(targetModuleFolderPath, "Views");
            EnsureTargetFolderExists(targetPrefabFolderPath);

            string templateViewPrefabPath = CombinePaths(BasePath, "Template", "TemplateScreen", "Views", "TemplateView.prefab");
            string targetPrefabPath = CombinePaths(targetPrefabFolderPath, $"{moduleName}View.prefab");

            AssetDatabase.CopyAsset(templateViewPrefabPath, targetPrefabPath);
            AssetDatabase.Refresh();

            GameObject prefabContents = PrefabUtility.LoadPrefabContents(targetPrefabPath);
            if (prefabContents != null)
            {
                var components = prefabContents.GetComponentsInChildren<Component>(true);
                foreach (var comp in components)
                {
                    if (comp == null)
                    {
                        string newScriptPath = CombinePaths(targetModuleFolderPath, "Scripts",
                            $"{moduleName}ScreenView.cs");
                        MonoScript newScript = AssetDatabase.LoadAssetAtPath<MonoScript>(newScriptPath);

                        if (newScript != null)
                        {
                            Type newComponentType = newScript.GetClass();
                            if (newComponentType != null)
                            {
                                prefabContents.AddComponent(newComponentType);
                                RemoveMissingScripts(prefabContents);
                            }
                            else
                                Debug.LogError($"Failed to get Type from MonoScript at {newScriptPath}");
                        }
                        else
                            Debug.LogError($"Failed to load new script at {newScriptPath}");

                        break;
                    }
                }

                prefabContents.name = $"{moduleName}View";

                PrefabUtility.SaveAsPrefabAsset(prefabContents, targetPrefabPath);
                PrefabUtility.UnloadPrefabContents(prefabContents);
            }
            else
                Debug.LogError("Failed to load prefab contents at " + targetPrefabPath);
        }

        private static void RemoveMissingScripts(GameObject go)
        {
            var serializedObject = new SerializedObject(go);
            var prop = serializedObject.FindProperty("m_Component");

            int r = 0;
            for (int i = 0; i < prop.arraySize; i++)
            {
                var component = prop.GetArrayElementAtIndex(i - r);
                var obj = component.objectReferenceValue;
                if (obj == null)
                {
                    prop.DeleteArrayElementAtIndex(i - r);
                    r++;
                }
            }
            serializedObject.ApplyModifiedProperties();

            foreach (Transform child in go.transform) 
                RemoveMissingScripts(child.gameObject);
        }

        private static string CombinePaths(params string[] paths) => 
            string.Join("/", paths.Select(p => p.Trim('/', '\\')));
    }
}
