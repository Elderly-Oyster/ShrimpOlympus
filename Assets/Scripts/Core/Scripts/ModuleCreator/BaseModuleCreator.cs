using System.IO;
using UnityEditor;

namespace Core.Scripts.ModuleCreator
{
    public class BaseModuleCreator
    {
        public const string BasePath = "Assets/Scripts/Modules";
        public const float GUISpacing = 10f;

        public string AdditionalFolderPath { get; private set; }
        public string BaseFolderPath { get; private set; }
        public string TestFolderPath { get; private set; }
        public string TemplateFolderPath { get; private set; }
        public string TemplateModuleFolderPath { get; private set; }

        public enum FolderType { Additional, Base, Test }

        public void InitializePaths()
        {
            AdditionalFolderPath = Path.Combine(BasePath, "Additional");
            BaseFolderPath = Path.Combine(BasePath, "Base");
            TestFolderPath = Path.Combine(BasePath, "Test");
            TemplateFolderPath = Path.Combine(BasePath, "Template", "TemplateScreen", "Scripts");
            TemplateModuleFolderPath = Path.Combine(BasePath, "Template", "TemplateScreen");
        }

        public void EnsureSubfoldersExist()
        {
            CreateFolderIfNotExists(AdditionalFolderPath);
            CreateFolderIfNotExists(BaseFolderPath);
            CreateFolderIfNotExists(TestFolderPath);
        }

        private static void CreateFolderIfNotExists(string folderPath)
        {
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                string parentFolder = Path.GetDirectoryName(folderPath);
                string newFolderName = Path.GetFileName(folderPath);
                AssetDatabase.CreateFolder(parentFolder, newFolderName);
            }
        }

        public static void EnsureTargetFolderExists(string targetFolderPath)
        {
            if (!AssetDatabase.IsValidFolder(targetFolderPath))
            {
                string parentFolder = Path.GetDirectoryName(targetFolderPath);
                string newFolderName = Path.GetFileName(targetFolderPath);
                AssetDatabase.CreateFolder(parentFolder, newFolderName);
            }
        }

        public static bool IsValidModuleName(string moduleName) =>
            !string.IsNullOrWhiteSpace(moduleName) && !moduleName.Contains(" ");

        public string GetSelectedFolderPath(FolderType selectedFolder)
        {
            return selectedFolder switch
            {
                FolderType.Additional => AdditionalFolderPath,
                FolderType.Base => BaseFolderPath,
                FolderType.Test => TestFolderPath,
                _ => BaseFolderPath
            };
        }
    }
}