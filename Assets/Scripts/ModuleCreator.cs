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

        CreateScript(targetFolderPath, $"{moduleName}ScreenInstaller.cs", 
            GetTemplateContent("NewModuleScreenInstaller.cs", moduleName));
        CreateScript(targetFolderPath, $"{moduleName}ScreenPresenter.cs", 
            GetTemplateContent("NewModuleScreenPresenter.cs", moduleName));
        CreateScript(targetFolderPath, $"{moduleName}ScreenView.cs", 
            GetTemplateContent("NewModuleScreenView.cs", moduleName));
        CreateScript(targetFolderPath, $"{moduleName}ScreenModel.cs", 
            GetTemplateContent("NewModuleScreenModel.cs", moduleName));

        AssetDatabase.Refresh();
    }

    string GetTemplateContent(string templateFileName, string moduleName)
    {
        string templateFolderPath = "Assets/Scripts/Modules/Base/NewBaseScreen";
        string templateFilePath = Path.Combine(templateFolderPath, templateFileName);
        string templateContent = File.ReadAllText(templateFilePath);

        templateContent = templateContent.Replace("NewModuleScreen", moduleName);

        return templateContent;
    }

    void CreateScript(string folderPath, string fileName, string scriptContent)
    {
        string filePath = Path.Combine(folderPath, fileName);

        File.WriteAllText(filePath, scriptContent);
    }
}
