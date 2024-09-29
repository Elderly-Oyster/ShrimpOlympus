using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Core.Scripts.ModuleCreator
{
    public class AsmdefCreator
    {
        private string _templateModuleFolderPath;

        public AsmdefCreator(string templateModuleFolderPath) => 
            _templateModuleFolderPath = templateModuleFolderPath;

        public void CreateAsmdefFile(string targetFolderPath, string moduleName)
        {
            string templateAsmdefPath = Path.Combine(_templateModuleFolderPath, "TemplateScreen.asmdef");
            string targetAsmdefPath = Path.Combine(targetFolderPath, $"{moduleName}Screen.asmdef");
            CopyAndAdjustAsmdef(templateAsmdefPath, targetAsmdefPath, moduleName);
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

        private static string ReadTemplateFile(string templateFilePath)
        {
            if (!File.Exists(templateFilePath))
            {
                Debug.LogError($"Template file not found at {templateFilePath}");
                return null;
            }

            return File.ReadAllText(templateFilePath);
        }

        private static string AdjustAsmdefContent(string content, string moduleName) =>
            Regex.Replace(content, @"""name"":\s*""[^""]+""", $@"""name"": ""{moduleName}Screen""");

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
