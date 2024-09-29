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

        private readonly List<string> _requiredTemplates = new List<string>
        {
            "TemplateScreenInstaller.cs",
            "TemplateScreenPresenter.cs",
            "TemplateScreenView.cs",
            "TemplateScreenModel.cs"
        };

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
            _templateModuleFolderPath = Path.Combine(BasePath, "Template", "TemplateScreen");
        }

        private static void EnsureSubfoldersExist()
        {
            CreateFolderIfNotExists("Additional");
            CreateFolderIfNotExists("Base");
            CreateFolderIfNotExists("Test");
        }

        private static void CreateFolderIfNotExists(string folderName)
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
            EnsureModuleFolders(targetFolderPath);

            string scriptsFolderPath = Path.Combine(targetFolderPath, "Scripts");

            if (_createAsmdef) 
                CreateAsmdefFile(targetFolderPath, moduleName);

            CreateSelectedScripts(scriptsFolderPath, moduleName);

            AssetDatabase.Refresh();

            DisplaySuccessMessage(moduleName);
        }

        private static void EnsureModuleFolders(string targetFolderPath)
        {
            EnsureTargetFolderExists(targetFolderPath);
            string scriptsFolderPath = Path.Combine(targetFolderPath, "Scripts");
            EnsureTargetFolderExists(scriptsFolderPath);
        }

        private void CreateAsmdefFile(string targetFolderPath, string moduleName)
        {
            string templateAsmdefPath = Path.Combine(_templateModuleFolderPath, "TemplateScreen.asmdef");
            string targetAsmdefPath = Path.Combine(targetFolderPath, $"{moduleName}Screen.asmdef");
            CopyAndAdjustAsmdef(templateAsmdefPath, targetAsmdefPath, moduleName);
        }

        private static void DisplaySuccessMessage(string moduleName)
        {
            EditorUtility.DisplayDialog("Success",
                $"Module {moduleName} created successfully.", "OK");
        }

        private static void EnsureTargetFolderExists(string targetFolderPath)
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
            string content = ReadTemplateFile(templateFilePath);
            if (content == null)
                return null;

            string moduleNameLower = char.ToLower(moduleName[0]) + moduleName.Substring(1);
            content = ReplaceNamespace(content, moduleName);
            content = ReplaceTemplateOccurrences(content, moduleName, moduleNameLower);
            return content;
        }

        private static string ReadTemplateFile(string templateFilePath)
        {
            if (!File.Exists(templateFilePath))
            {
                Debug.LogError($"Template file not found at {templateFilePath}");
                return null;
            }

            return File.ReadAllText(templateFilePath);
        }

        private string ReplaceNamespace(string content, string moduleName)
        {
            string namespaceReplacement = $"namespace Modules.{_selectedFolder}.{moduleName}Screen.Scripts";
            return Regex.Replace(content, @"namespace\s+[\w\.]+", namespaceReplacement);
        }

        private static string ReplaceTemplateOccurrences(string content, string moduleName, string moduleNameLower)
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

        private static void CreateScript(string folderPath, string fileName, string scriptContent)
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

            WriteToFile(filePath, scriptContent);
        }

        private static void WriteToFile(string filePath, string content)
        {
            try
            {
                File.WriteAllText(filePath, content);
                Debug.Log($"File created at {filePath}");
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
                    $"Template asmdef file not found at {templateAsmdefPath}.\n\nCannot create asmdef file.",
                    "OK");
                return;
            }

            content = AdjustAsmdefContent(content, moduleName);

            WriteToFile(targetAsmdefPath, content);
        }

        private static string AdjustAsmdefContent(string content, string moduleName) => 
            Regex.Replace(content, @"""name"":\s*""[^""]+""", $@"""name"": ""{moduleName}Screen""");
    }
}