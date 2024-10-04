using System;
using Modules.Template.TemplateScreen.Scripts;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor
{
    public static class PrefabCreator
    {
        public static void CreatePrefabForModule(string moduleName, string targetModuleFolderPath)
        {
            Debug.Log($"CreatePrefabForModule called with moduleName: {moduleName}," +
                      $" targetModuleFolderPath: {targetModuleFolderPath}");

            string targetPrefabPath = CopyTemplatePrefab(moduleName, targetModuleFolderPath);
            if (string.IsNullOrEmpty(targetPrefabPath))
            {
                Debug.LogError("Target prefab path is null or empty.");
                return;
            }

            GameObject prefabContents = PrefabUtility.LoadPrefabContents(targetPrefabPath);

            if (prefabContents == null)
            {
                Debug.LogError("Failed to load prefab contents at " + targetPrefabPath);
                return;
            }

            ReplaceTemplateScreenViewScript(prefabContents, moduleName);
            SaveAndUnloadPrefab(prefabContents, targetPrefabPath, moduleName);
            Debug.Log($"Prefab for module {moduleName} created successfully.");
        }

        private static string CopyTemplatePrefab(string moduleName, string targetModuleFolderPath)
        {
            Debug.Log("Module name: " + moduleName);
            Debug.Log("Target module folder path: " + targetModuleFolderPath);

            string targetPrefabFolderPath = PathManager.CombinePaths(targetModuleFolderPath, "Views");
            ModuleGenerator.EnsureTargetFolderExists(targetPrefabFolderPath);

            string templateViewPrefabPath = PathManager.TemplateViewPrefabPath;
            string targetPrefabPath = PathManager.CombinePaths(targetPrefabFolderPath, $"{moduleName}View.prefab");

            Debug.Log($"Copying prefab from '{templateViewPrefabPath}' to '{targetPrefabPath}'");

            bool copyResult = AssetDatabase.CopyAsset(templateViewPrefabPath, targetPrefabPath);
            if (!copyResult)
            {
                Debug.LogError($"Failed to copy prefab from '{templateViewPrefabPath}' to '{targetPrefabPath}'.");
                return null;
            }
            AssetDatabase.ImportAsset(targetPrefabPath, ImportAssetOptions.ForceUpdate);

            return targetPrefabPath;
        }

        private static void ReplaceTemplateScreenViewScript(GameObject prefabContents, string moduleName)
        {
            var templateViewComponent = prefabContents.GetComponent<TemplateScreenView>();
            if (templateViewComponent == null)
            {
                Debug.LogError("TemplateScreenView component not found in prefab.");
                return;
            }

            string namespaceName = $"Modules.{moduleName}Screen.Scripts";
            string className = $"{namespaceName}.{moduleName}ScreenView";

            Type newComponentType = GetTypeByName(className);

            if (newComponentType == null)
            {
                Debug.LogError($"Failed to find Type '{className}' after scripts reload.");
                return;
            }

            ReplaceComponent(prefabContents, templateViewComponent, newComponentType);
        }

        private static Type GetTypeByName(string className)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(className);
                if (type != null)
                    return type;
            }
            return null;
        }

        private static void ReplaceComponent(GameObject gameObject, Component oldComponent, Type newComponentType)
        {
            var newComponent = gameObject.AddComponent(newComponentType);

            if (newComponent == null)
            {
                Debug.LogError("Failed to add new component.");
                return;
            }

            EditorUtility.CopySerialized(oldComponent, newComponent);

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
