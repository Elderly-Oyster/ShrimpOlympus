using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Core.ModuleCreator
{
    public class ModuleCreator : EditorWindow
    {
        private enum FolderType { Additional, Base, Test }

        private const string BasePath = "Assets/Scripts/Modules";
        private const float GUISpacing = 10f;

        private string _moduleName = "NewModule";
        private bool _createInstaller = true;
        private bool _createPresenter = true;
        private bool _createView = true;
        private bool _createModel = true;

        private FolderType _selectedFolder = FolderType.Base;

        private string _additionalFolderPath;
        private string _baseFolderPath;
        private string _testFolderPath;
        private string _templateFolderPath;

        private readonly List<string> _requiredTemplates = new List<string>
        {
            "TemplateScreenInstaller.cs",
            "TemplateScreenPresenter.cs",
            "TemplateScreenView.cs",
            "TemplateScreenModel.cs"
        };

        [MenuItem("Tools/Create Module")]
        public static void ShowWindow() => GetWindow<ModuleCreator>("Create Module");

        private bool IsValidModuleName(string moduleName) =>
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
            if (GUILayout.Button("Create Module"))
            {
                if (IsValidModuleName(_moduleName))
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

        private void InitializePaths()
        {
            _additionalFolderPath = Path.Combine(BasePath, "Additional");
            _baseFolderPath = Path.Combine(BasePath, "Base");
            _testFolderPath = Path.Combine(BasePath, "Test");
            _templateFolderPath = Path.Combine(BasePath, "Template", "TemplateScreen");
        }

        private void EnsureSubfoldersExist()
        {
            CreateFolderIfNotExists("Additional");
            CreateFolderIfNotExists("Base");
            CreateFolderIfNotExists("Test");
        }

        private void CreateFolderIfNotExists(string folderName)
        {
            string folderPath = Path.Combine(BasePath, folderName);
            if (!AssetDatabase.IsValidFolder(folderPath))
                AssetDatabase.CreateFolder(BasePath, folderName);
        }

        private bool AreTemplatesAvailable()
        {
            if (!AssetDatabase.IsValidFolder(_templateFolderPath))
            {
                EditorUtility.DisplayDialog("Missing Template Folder",
                    $"Template folder not found at {_templateFolderPath}.\n\nModule creation aborted.",
                    "OK");
                return false;
            }

            var missingTemplates = _requiredTemplates.Where(template =>
                !File.Exists(Path.Combine(_templateFolderPath, template))).ToList();

            if (missingTemplates.Any())
            {
                string missing = string.Join("\n", missingTemplates);
                EditorUtility.DisplayDialog("Missing Templates",
                    $"The following template files are missing:\n{missing}\n\nModule creation aborted.",
                    "OK");
                return false;
            }

            return true;
        }

        private string GetSelectedFolderPath()
        {
            return _selectedFolder switch
            {
                FolderType.Additional => _additionalFolderPath,
                FolderType.Base => _baseFolderPath,
                FolderType.Test => _testFolderPath,
                _ => _baseFolderPath
            };
        }

        private void CreateModuleFiles(string moduleName)
        {
            string selectedFolderPath = GetSelectedFolderPath();
            string targetFolderPath = Path.Combine(selectedFolderPath, $"{moduleName}Screen");
            EnsureTargetFolderExists(targetFolderPath);
            CreateSelectedScripts(targetFolderPath, moduleName);
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success",
                $"Module {moduleName} created successfully.", "OK");
        }

        private void EnsureTargetFolderExists(string targetFolderPath)
        {
            if (!AssetDatabase.IsValidFolder(targetFolderPath))
                AssetDatabase.CreateFolder(Path.GetDirectoryName(targetFolderPath), Path.GetFileName(targetFolderPath));
        }

        private void CreateSelectedScripts(string targetFolderPath, string moduleName)
        {
            if (_createInstaller)
                CreateScript(targetFolderPath, $"{moduleName}ScreenInstaller.cs",
                    GetTemplateContent("TemplateScreenInstaller.cs", moduleName));
            if (_createPresenter)
                CreateScript(targetFolderPath, $"{moduleName}ScreenPresenter.cs",
                    GetTemplateContent("TemplateScreenPresenter.cs", moduleName));
            if (_createView)
                CreateScript(targetFolderPath, $"{moduleName}ScreenView.cs",
                    GetTemplateContent("TemplateScreenView.cs", moduleName));
            if (_createModel)
                CreateScript(targetFolderPath, $"{moduleName}ScreenModel.cs",
                    GetTemplateContent("TemplateScreenModel.cs", moduleName));
        }

        private string GetTemplateContent(string templateFileName, string moduleName)
        {
            string templateFilePath = Path.Combine(_templateFolderPath, templateFileName);
            string content = File.ReadAllText(templateFilePath);

            string moduleNameLower = char.ToLower(moduleName[0]) + moduleName.Substring(1);
            string namespaceReplacement = $"namespace Modules.{_selectedFolder}.{moduleName}Screen";
            content = Regex.Replace(content, @"namespace\s+[\w\.]+", namespaceReplacement);

            content = ReplaceTemplateOccurrences(content, moduleName, moduleNameLower);
            return content;
        }

        private string ReplaceTemplateOccurrences(string content, string moduleName, string moduleNameLower)
        {
            return Regex.Replace(content, @"(_?)(template)", match =>
            {
                string prefix = match.Groups[1].Value;
                string templateWord = match.Groups[2].Value;

                if (char.IsUpper(templateWord[0]))
                    return prefix + moduleName;
                else
                    return prefix + moduleNameLower;
            }, RegexOptions.IgnoreCase);
        }

        private void CreateScript(string folderPath, string fileName, string scriptContent)
        {
            if (string.IsNullOrEmpty(scriptContent))
            {
                Debug.LogError($"Script content is null or empty for {fileName}");
                return;
            }

            string filePath = Path.Combine(folderPath, fileName);

            if (File.Exists(filePath))
            {
                if (!EditorUtility.DisplayDialog(
                    "File Exists",
                    $"File {fileName} already exists. Overwrite?",
                    "Yes",
                    "No"))
                {
                    Debug.Log($"Skipped creating file: {fileName}");
                    return;
                }
            }

            try
            {
                File.WriteAllText(filePath, scriptContent);
                Debug.Log($"Script created at {filePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error writing file {Path.GetFileName(filePath)}: {ex.Message}");
            }
        }
    }
}
