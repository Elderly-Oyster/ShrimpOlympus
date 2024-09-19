using UnityEngine;
using UnityEditor;
using System.IO;

public class ModuleCreator : EditorWindow
{
    private string _moduleName = "NewModule";

    [MenuItem("Tools/Create Module")]
    public static void ShowWindow()
    {
        GetWindow<ModuleCreator>("Create Module");
    }

    void OnGUI()
    {
        GUILayout.Label("Module Creator", EditorStyles.boldLabel);
        _moduleName = EditorGUILayout.TextField("Module Name", _moduleName);

        if (GUILayout.Button("Create Module"))
        {
            CreateModuleFiles(_moduleName);
        }
    }

    void CreateModuleFiles(string moduleName)
    {
        string targetFolderPath = $"Assets/Scripts/Modules/Base/{moduleName}Screen";

        if (!AssetDatabase.IsValidFolder(targetFolderPath))
        {
            string baseFolderPath = "Assets/Scripts/Modules/Base";
            if (!AssetDatabase.IsValidFolder(baseFolderPath))
            {
                AssetDatabase.CreateFolder("Assets/Scripts/Modules", "Base");
            }
            AssetDatabase.CreateFolder(baseFolderPath, $"{moduleName}Screen");
        }

        // Create the scripts, logging each step
        Debug.Log($"Creating scripts for module: {moduleName}");
        CreateScript(targetFolderPath, $"{moduleName}ScreenInstaller.cs", GetTemplateContent("TemplateScreenInstaller.cs", moduleName));
        CreateScript(targetFolderPath, $"{moduleName}ScreenPresenter.cs", GetTemplateContent("TemplateScreenPresenter.cs", moduleName));
        CreateScript(targetFolderPath, $"{moduleName}ScreenView.cs", GetTemplateContent("TemplateScreenView.cs", moduleName));
        CreateScript(targetFolderPath, $"{moduleName}ScreenModel.cs", GetTemplateContent("TemplateScreenModel.cs", moduleName));

        AssetDatabase.Refresh();
    }

    string GetTemplateContent(string templateFileName, string moduleName)
    {
        string templateFolderPath = "Assets/Scripts/Modules/Base/TemplateScreen";
        string templateFilePath = Path.Combine(templateFolderPath, templateFileName);
        string templateContent = File.ReadAllText(templateFilePath);

        templateContent = templateContent.Replace("TemplateInstaller", $"{moduleName}ScreenInstaller");
        templateContent = templateContent.Replace("TemplatePresenter", $"{moduleName}ScreenPresenter");
        templateContent = templateContent.Replace("TemplateView", $"{moduleName}ScreenView");
        templateContent = templateContent.Replace("TemplateModel", $"{moduleName}ScreenModel");
        templateContent = templateContent.Replace("TemplateScreen", $"{moduleName}Screen");

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

        // Write the script file, logging the file content before writing
        Debug.Log($"Writing script to {filePath} with content:\n{scriptContent}");
        File.WriteAllText(filePath, scriptContent);
    }
}
