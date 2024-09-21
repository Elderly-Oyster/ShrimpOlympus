using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Core.ModuleCreator
{
    public class ModuleCreator : EditorWindow
    {
        private const string BaseFolderPath = "Assets/Scripts/Modules/Base";
        private string _moduleName = "NewModule";

        [MenuItem("Tools/Create Module")]
        public static void ShowWindow() => GetWindow<ModuleCreator>("Create Module");

        bool IsValidModuleName(string moduleName) => 
            !string.IsNullOrWhiteSpace(moduleName) && !moduleName.Contains(" ");

        void OnGUI()
        {
            GUILayout.Label("Module Creator", EditorStyles.boldLabel);
            _moduleName = EditorGUILayout.TextField("Module Name", _moduleName);

            if (GUILayout.Button("Create Module")) 
            {
                if (IsValidModuleName(_moduleName))
                    CreateModuleFiles(_moduleName);
                else
                    EditorUtility.DisplayDialog("Invalid Name", "Module name cannot be empty or contain spaces.", "OK");
            }
        }

        private void CreateModuleFiles(string moduleName)
        {
            try
            {
                string targetFolderPath = Path.Combine(BaseFolderPath, $"{moduleName}Screen");

                if (!AssetDatabase.IsValidFolder(targetFolderPath))
                {
                    if (!AssetDatabase.IsValidFolder(BaseFolderPath))
                        AssetDatabase.CreateFolder("Assets/Scripts/Modules", "Base");
                    AssetDatabase.CreateFolder(BaseFolderPath, $"{moduleName}Screen");
                }

                Debug.Log($"Creating scripts for module: {moduleName}");
                CreateScript(targetFolderPath, $"{moduleName}ScreenInstaller.cs",
                    GetTemplateContent("TemplateScreenInstaller.cs", moduleName));
                CreateScript(targetFolderPath, $"{moduleName}ScreenPresenter.cs",
                    GetTemplateContent("TemplateScreenPresenter.cs", moduleName));
                CreateScript(targetFolderPath, $"{moduleName}ScreenView.cs",
                    GetTemplateContent("TemplateScreenView.cs", moduleName));
                CreateScript(targetFolderPath, $"{moduleName}ScreenModel.cs",
                    GetTemplateContent("TemplateScreenModel.cs", moduleName));

                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("Success", $"Module {moduleName} created successfully.", "OK");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error creating module {moduleName}: {ex.Message}");
                EditorUtility.DisplayDialog("Error", $"Failed to create module: {ex.Message}", "OK");
            }
        }

        string GetTemplateContent(string templateFileName, string moduleName)
        {
            string templateFolderPath = Path.Combine(BaseFolderPath, "TemplateScreen");
            string templateFilePath = Path.Combine(templateFolderPath, templateFileName);

            if (!File.Exists(templateFilePath))
            {
                Debug.LogError($"Template file not found: {templateFilePath}");
                EditorUtility.DisplayDialog("Error", $"Template file not found: {templateFileName}", "OK");
                return string.Empty;
            }

            string templateContent = File.ReadAllText(templateFilePath);

            templateContent = templateContent.Replace("TemplateInstaller", 
                $"{moduleName}ScreenInstaller");
            templateContent = templateContent.Replace("TemplatePresenter", 
                $"{moduleName}ScreenPresenter");
            templateContent = templateContent.Replace("TemplateView", 
                $"{moduleName}ScreenView");
            templateContent = templateContent.Replace("TemplateModel",
                $"{moduleName}ScreenModel");
            templateContent = templateContent.Replace("TemplateScreen", 
                $"{moduleName}Screen");

            Debug.Log($"Replaced template content for {moduleName}:\n{templateContent}");

            return templateContent;
        }

        void CreateScript(string folderPath, string fileName, string scriptContent)
        {
            if (string.IsNullOrEmpty(scriptContent))
            {
                Debug.LogError($"Script content is null or empty for {fileName}");
                return;
            }

            string filePath = Path.Combine(folderPath, fileName);

            if (File.Exists(filePath))
            {
                bool overwrite = EditorUtility.DisplayDialog(
                    "File Exists",
                    $"File {fileName} already exists. Overwrite?",
                    "Yes",
                    "No"
                );

                if (!overwrite)
                {
                    Debug.Log($"Skipped creating file: {fileName}");
                    return;
                }
            }

            try
            {
                Debug.Log($"Writing script to {filePath} with content:\n{scriptContent}");
                File.WriteAllText(filePath, scriptContent);
            }
            catch (Exception ex) 
            { Debug.LogError($"Error writing file {fileName}: {ex.Message}"); }
        }
    }
}
