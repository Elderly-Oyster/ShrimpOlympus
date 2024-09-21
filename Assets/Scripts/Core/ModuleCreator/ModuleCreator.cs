using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Core.ModuleCreator
{
    public class ModuleCreator : EditorWindow
    {
        private enum FolderType { Additional, Base, Test }

        private const string BasePath = "Assets/Scripts/Modules";

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
            DrawModuleNameField();
            DrawFolderSelection();
            DrawScriptSelection();
            DrawCreateButton();
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
            CreateTemplateFolder();
        }

        private void CreateFolderIfNotExists(string folderName)
        {
            string folderPath = Path.Combine(BasePath, folderName);
            if (!AssetDatabase.IsValidFolder(folderPath))
                AssetDatabase.CreateFolder(Path.Combine(BasePath), folderName);
        }

        private void CreateTemplateFolder()
        {
            string templateParentPath = Path.Combine(BasePath, "Template");
            if (!AssetDatabase.IsValidFolder(templateParentPath))
                AssetDatabase.CreateFolder(Path.Combine(BasePath), "Template");

            if (!AssetDatabase.IsValidFolder(_templateFolderPath))
                AssetDatabase.CreateFolder(templateParentPath, "TemplateScreen");
        }

        private void DrawModuleNameField()
        {
            GUILayout.Label("Module Creator", EditorStyles.boldLabel);
            _moduleName = EditorGUILayout.TextField("Module Name", _moduleName);
        }

        private void DrawFolderSelection()
        {
            GUILayout.Space(10);
            _selectedFolder = (FolderType)EditorGUILayout.EnumPopup("Select Folder", _selectedFolder);
        }

        private void DrawScriptSelection()
        {
            GUILayout.Space(10);
            GUILayout.Label("Select Scripts to Create", EditorStyles.boldLabel);
            _createInstaller = EditorGUILayout.Toggle("Installer", _createInstaller);
            _createPresenter = EditorGUILayout.Toggle("Presenter", _createPresenter);
            _createView = EditorGUILayout.Toggle("View", _createView);
            _createModel = EditorGUILayout.Toggle("Model", _createModel);
        }

        private void DrawCreateButton()
        {
            GUILayout.Space(10);
            if (GUILayout.Button("Create Module"))
            {
                if (IsValidModuleName(_moduleName))
                    CreateModuleFiles(_moduleName);
                else
                    EditorUtility.DisplayDialog("Invalid Name", 
                        "Module name cannot be empty or contain spaces.", "OK");
            }
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
            try
            {
                string selectedFolderPath = GetSelectedFolderPath();
                string targetFolderPath = Path.Combine(selectedFolderPath, $"{moduleName}Screen");

                EnsureTargetFolderExists(targetFolderPath, moduleName);
                CreateSelectedScripts(targetFolderPath, moduleName);

                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("Success", $"Module {moduleName} created successfully.", "OK");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error creating module {moduleName}: {ex.Message}");
                EditorUtility.DisplayDialog("Error", $"Failed to create module: {ex.Message}", "OK");
            }
        }

        private void EnsureTargetFolderExists(string targetFolderPath, string moduleName)
        {
            if (!AssetDatabase.IsValidFolder(targetFolderPath))
                AssetDatabase.CreateFolder(GetSelectedFolderPath(), $"{moduleName}Screen");
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

            if (!File.Exists(templateFilePath))
            {
                Debug.LogError($"Template file not found: {templateFilePath}");
                EditorUtility.DisplayDialog("Error", $"Template file not found: {templateFileName}", "OK");
                return string.Empty;
            }

            string templateContent = File.ReadAllText(templateFilePath);
            string folderType = _selectedFolder.ToString();
            string moduleNameLower = Char.ToLower(moduleName[0]) + moduleName.Substring(1);

            templateContent = ReplaceNamespace(templateContent, folderType, moduleName);
            templateContent = ReplaceClassNames(templateContent, moduleName);
            templateContent = ReplaceVariableNames(templateContent, moduleName, moduleNameLower);

            Debug.Log($"Replaced template content for {moduleName}:\n{templateContent}");
            return templateContent;
        }

        private string ReplaceNamespace(string content, string folderType, string moduleName)
        {
            string originalNamespace = "namespace Modules.Template.TemplateScreen";
            string newNamespace = $"namespace Modules.{folderType}.{moduleName}Screen";
            return content.Replace(originalNamespace, newNamespace);
        }

        private string ReplaceClassNames(string content, string moduleName)
        {
            var classReplacements = new Dictionary<string, string>
            {
                { "TemplateInstaller", $"{moduleName}ScreenInstaller" },
                { "TemplatePresenter", $"{moduleName}ScreenPresenter" },
                { "TemplateView", $"{moduleName}ScreenView" },
                { "TemplateModel", $"{moduleName}ScreenModel" }
            };

            foreach (var kvp in classReplacements) 
                content = ReplaceWholeWord(content, kvp.Key, kvp.Value);

            return content;
        }

        private string ReplaceVariableNames(string content, string moduleName, string moduleNameLower)
        {
            var variableReplacementsWithUnderscore = new Dictionary<string, string>
            {
                { "_TemplateInstaller", $"_{moduleNameLower}ScreenInstaller" },
                { "_TemplatePresenter", $"_{moduleNameLower}ScreenPresenter" },
                { "_TemplateView", $"_{moduleNameLower}ScreenView" },
                { "_TemplateModel", $"_{moduleNameLower}ScreenModel" }
            };

            foreach (var kvp in variableReplacementsWithUnderscore)
                content = ReplaceWholeWord(content, kvp.Key, kvp.Value);

            var variableReplacementsWithoutUnderscore = new Dictionary<string, string>
            {
                { "TemplateInstaller", $"{moduleName}ScreenInstaller" },
                { "TemplatePresenter", $"{moduleName}ScreenPresenter" },
                { "TemplateView", $"{moduleName}ScreenView" },
                { "TemplateModel", $"{moduleName}ScreenModel" },
                { "newModuleScreenView", $"new{moduleName}ScreenView" }
            };

            foreach (var kvp in variableReplacementsWithoutUnderscore)
                content = ReplaceWholeWord(content, kvp.Key, kvp.Value);

            return content;
        }

        private string ReplaceWholeWord(string input, string word, string replacement) => 
            System.Text.RegularExpressions.Regex.Replace(input, $@"\b{word}\b", replacement);

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
                if (!ConfirmOverwrite(fileName))
                {
                    Debug.Log($"Skipped creating file: {fileName}");
                    return;
                }
            }

            WriteScriptFile(filePath, scriptContent);
        }

        private bool ConfirmOverwrite(string fileName)
        {
            return EditorUtility.DisplayDialog(
                "File Exists",
                $"File {fileName} already exists. Overwrite?",
                "Yes",
                "No"
            );
        }

        private void WriteScriptFile(string filePath, string scriptContent)
        {
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
