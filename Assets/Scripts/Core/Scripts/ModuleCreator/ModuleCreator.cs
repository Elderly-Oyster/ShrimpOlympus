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
        private bool _createAsmdef = true; // Добавлено: возможность выбора создания asmdef

        private string _additionalFolderPath;
        private string _baseFolderPath;
        private string _testFolderPath;
        private string _templateFolderPath;
        private string _templateModuleFolderPath; // Путь к папке модуля шаблона

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
            _createAsmdef = EditorGUILayout.Toggle("Create asmdef", _createAsmdef);

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
            _templateFolderPath = Path.Combine(BasePath, "Template", "TemplateScreen", "Scripts");
            _templateModuleFolderPath = Path.Combine(BasePath, "Template", "TemplateScreen"); // Путь к папке модуля шаблона
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

            if (_createAsmdef)
            {
                string templateAsmdefPath = Path.Combine(_templateModuleFolderPath, "TemplateScreen.asmdef");
                if (!File.Exists(templateAsmdefPath))
                {
                    EditorUtility.DisplayDialog("Missing asmdef Template",
                        $"Template asmdef file not found at {templateAsmdefPath}.\n\nModule creation aborted.",
                        "OK");
                    return false;
                }
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

            string scriptsFolderPath = Path.Combine(targetFolderPath, "Scripts");
            EnsureTargetFolderExists(scriptsFolderPath);

            if (_createAsmdef)
            {
                string templateAsmdefPath = Path.Combine(_templateModuleFolderPath, "TemplateScreen.asmdef");
                string targetAsmdefPath = Path.Combine(targetFolderPath, $"{moduleName}Screen.asmdef");
                CopyAndAdjustAsmdef(templateAsmdefPath, targetAsmdefPath, moduleName);
            }

            CreateSelectedScripts(scriptsFolderPath, moduleName);

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success",
                $"Module {moduleName} created successfully.", "OK");
        }

        private void EnsureTargetFolderExists(string targetFolderPath)
        {
            if (!AssetDatabase.IsValidFolder(targetFolderPath))
            {
                string parentFolder = Path.GetDirectoryName(targetFolderPath);
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

        private void CopyAndAdjustAsmdef(string templateAsmdefPath, string targetAsmdefPath, string moduleName)
        {
            if (!File.Exists(templateAsmdefPath))
            {
                EditorUtility.DisplayDialog("Missing asmdef Template",
                    $"Template asmdef file not found at {templateAsmdefPath}.\n\nCannot create asmdef file.",
                    "OK");
                return;
            }

            string content = File.ReadAllText(templateAsmdefPath);
            content = Regex.Replace(content,
                @"""name"":\s*""[^""]+""", $@"""name"": ""{moduleName}Screen""");

            try
            {
                File.WriteAllText(targetAsmdefPath, content);
                Debug.Log($"Asmdef file created at {targetAsmdefPath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error writing asmdef file: {ex.Message}");
            }
        }
    }
}
