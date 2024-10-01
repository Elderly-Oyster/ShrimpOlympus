using System;
using System.IO;
using Modules.Template.TemplateScreen.Scripts;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Core.Scripts.ModuleCreator.Scripts
{
    [InitializeOnLoad]
    public static class PrefabCreator
    {
        static PrefabCreator() => 
            CompilationPipeline.compilationFinished += OnCompilationFinished;

        private static void OnCompilationFinished(object obj)
        {
            if (!EditorPrefs.GetBool(ModuleGenerator.ModuleCreationInProgressKey, false)) 
                return;

            string moduleName = EditorPrefs.GetString(ModuleGenerator.ModuleNameKey, "");
            string targetModuleFolderPath = EditorPrefs.
                GetString(ModuleGenerator.TargetModuleFolderPathKey, "");

            if (string.IsNullOrEmpty(moduleName) || string.IsNullOrEmpty(targetModuleFolderPath))
                return;

            CreatePrefabForModule(moduleName, targetModuleFolderPath);

            EditorPrefs.DeleteKey(ModuleGenerator.ModuleCreationInProgressKey);
            EditorPrefs.DeleteKey(ModuleGenerator.ModuleNameKey);
            EditorPrefs.DeleteKey(ModuleGenerator.TargetModuleFolderPathKey);

            EditorUtility.DisplayDialog("Module Creation Finished",
                "Scripts compiled and prefab created successfully.", "OK");

            CompilationPipeline.compilationFinished -= OnCompilationFinished;
        }

        private static void CreatePrefabForModule(string moduleName, string targetModuleFolderPath)
        {
            string targetPrefabPath = CopyTemplatePrefab(moduleName, targetModuleFolderPath);
            GameObject prefabContents = PrefabUtility.LoadPrefabContents(targetPrefabPath);

            if (prefabContents == null)
            {
                Debug.LogError("Failed to load prefab contents at " + targetPrefabPath);
                return;
            }

            ReplaceTemplateScreenViewScript(prefabContents, moduleName, targetModuleFolderPath);
            SaveAndUnloadPrefab(prefabContents, targetPrefabPath, moduleName);
        }

        private static string CopyTemplatePrefab(string moduleName, string targetModuleFolderPath)
        {
            string targetPrefabFolderPath = PathManager.CombinePaths(targetModuleFolderPath, "Views");
            ModuleGenerator.EnsureTargetFolderExists(targetPrefabFolderPath);

            string templateViewPrefabPath = PathManager.TemplateViewPrefabPath;
            string targetPrefabPath = PathManager.CombinePaths(targetPrefabFolderPath, $"{moduleName}View.prefab");

            AssetDatabase.CopyAsset(templateViewPrefabPath, targetPrefabPath);
            AssetDatabase.Refresh();

            return targetPrefabPath;
        }
        
        
        private static void ReplaceTemplateScreenViewScript(GameObject prefabContents, 
            string moduleName, string targetModuleFolderPath)
        {
            // Получаем компонент TemplateScreenView из префаба
            TemplateScreenView templateViewComponent = prefabContents.GetComponent<TemplateScreenView>();
            if (templateViewComponent == null)
            {
                Debug.LogError("TemplateScreenView component not found in prefab.");
                return;
            }

            // Формируем путь к новому скрипту
            string newScriptPath = PathManager.CombinePaths(targetModuleFolderPath, "Scripts", $"{moduleName}ScreenView.cs");
            newScriptPath = newScriptPath.Replace("\\", "/");
            Debug.Log("Attempting to load MonoScript at path: " + newScriptPath);

            // Обновляем AssetDatabase
            AssetDatabase.Refresh();

            // Проверяем, существует ли файл
            if (!File.Exists(newScriptPath))
            {
                Debug.LogError("File does not exist at path: " + newScriptPath);
                return;
            }

            // Пытаемся загрузить MonoScript
            MonoScript newScript = AssetDatabase.LoadAssetAtPath<MonoScript>(newScriptPath);
            if (newScript == null)
            {
                Debug.LogError($"Failed to load MonoScript at {newScriptPath}");
                return;
            }

            // Получаем тип класса из MonoScript
            Type newComponentType = newScript.GetClass();
            if (newComponentType == null)
            {
                Debug.LogError($"Failed to get Type from MonoScript at {newScriptPath}");
                return;
            }

            // Заменяем компонент
            ReplaceComponent(prefabContents, templateViewComponent, newComponentType);
        }



        private static void ReplaceComponent(GameObject gameObject, MonoBehaviour oldComponent, Type newComponentType)
        {
            var newComponent = gameObject.AddComponent(newComponentType) as MonoBehaviour;

            string json = EditorJsonUtility.ToJson(oldComponent);
            EditorJsonUtility.FromJsonOverwrite(json, newComponent);

            Object.DestroyImmediate(oldComponent, true);
        }

        private static void SaveAndUnloadPrefab(GameObject prefabContents, string prefabPath, string moduleName)
        {
            prefabContents.name = $"{moduleName}View";
            PrefabUtility.SaveAsPrefabAsset(prefabContents, prefabPath);
            PrefabUtility.UnloadPrefabContents(prefabContents);
        }
    }
}
