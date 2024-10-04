using UnityEditor;

namespace Editor
{
    public static class PathManager
    {
        public static string AdditionalFolderPath { get; private set; }
        public static string BaseFolderPath { get; private set; }
        public static string TestFolderPath { get; private set; }
        public static string TemplateScriptsFolderPath { get; private set; }
        public static string TemplateModuleFolderPath { get; private set; }
        public static string TemplateViewsFolderPath { get; private set; }
        public static string TemplateViewPrefabPath { get; private set; }

        private const string BasePath = "Assets/Scripts/Modules";

        public static void InitializePaths()
        {
            AdditionalFolderPath = CombinePaths(BasePath, "Additional");
            BaseFolderPath = CombinePaths(BasePath, "Base");
            TestFolderPath = CombinePaths(BasePath, "Test");
            
            TemplateModuleFolderPath = CombinePaths(BasePath, "Template", "TemplateScreen");
            TemplateViewsFolderPath = CombinePaths(TemplateModuleFolderPath, "Views");
            TemplateScriptsFolderPath = CombinePaths(TemplateModuleFolderPath, "Scripts");
            TemplateViewPrefabPath = CombinePaths(TemplateViewsFolderPath, "TemplateScreenView.prefab");

            EnsureSubfoldersExist();
        }

        private static void EnsureSubfoldersExist()
        {
            CreateFolderIfNotExists(AdditionalFolderPath);
            CreateFolderIfNotExists(BaseFolderPath);
            CreateFolderIfNotExists(TestFolderPath);
        }

        public static string CombinePaths(params string[] paths) =>
            string.Join("/", paths).Replace("\\", "/");

        private static void CreateFolderIfNotExists(string folderPath)
        {
            folderPath = folderPath.Replace("\\", "/");
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                string parentFolder = System.IO.Path.GetDirectoryName(folderPath)?.Replace("\\", "/");
                string newFolderName = System.IO.Path.GetFileName(folderPath);
                AssetDatabase.CreateFolder(parentFolder, newFolderName);
            }
        }
    }
}