using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Core.Scripts.ModuleCreator
{
    public class ScriptModuleCreator
    {
        private readonly string _templateFolderPath;

        public ScriptModuleCreator(string templateFolderPath) => _templateFolderPath = templateFolderPath;

        public void CreateSelectedScripts(string folderPath, string moduleName, 
            BaseModuleCreator.FolderType selectedFolder, bool createInstaller, bool createPresenter, bool createView, bool createModel)
        {
            var scriptsToCreate = new List<(bool shouldCreate, string templateFile, string outputFile)>
            {
                (createInstaller, "TemplateScreenInstaller.cs", $"{moduleName}ScreenInstaller.cs"),
                (createPresenter, "TemplateScreenPresenter.cs", $"{moduleName}ScreenPresenter.cs"),
                (createView, "TemplateScreenView.cs", $"{moduleName}ScreenView.cs"),
                (createModel, "TemplateScreenModel.cs", $"{moduleName}ScreenModel.cs"),
            };

            foreach (var (shouldCreate, templateFile, outputFile) in scriptsToCreate)
            {
                if (shouldCreate)
                {
                    string content = GetTemplateContent(templateFile, moduleName, selectedFolder);
                    CreateScript(folderPath, outputFile, content);
                }
            }
        }

        private string GetTemplateContent(string templateFileName, string moduleName, 
            BaseModuleCreator.FolderType selectedFolder)
        {
            string templateFilePath = Path.Combine(_templateFolderPath, templateFileName);
            string content = ReadTemplateFile(templateFilePath);
            if (content == null)
                return null;

            string moduleNameLower = char.ToLower(moduleName[0]) + moduleName.Substring(1);

            content = Regex.Replace(content, @"namespace\s+[\w\.]+", 
                $"namespace Modules.{selectedFolder}.{moduleName}Screen.Scripts");

            content = content.Replace("TemplateScreenView", $"{moduleName}ScreenView");
            content = content.Replace("TemplateScreen", $"{moduleName}Screen");
            content = content.Replace("templateScreen", $"{moduleNameLower}Screen");
            content = content.Replace("Template", moduleName);
            content = content.Replace("template", moduleNameLower);

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

        private string ReplaceNamespace(string content, string moduleName, BaseModuleCreator.FolderType selectedFolder)
        {
            string namespaceReplacement = $"namespace Modules.{selectedFolder}.{moduleName}Screen.Scripts";
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
                        "File Already Exists",
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
    }
}
