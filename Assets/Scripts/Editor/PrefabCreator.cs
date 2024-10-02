using System;
using System.Linq;
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
            string targetPrefabPath = CopyTemplatePrefab(moduleName, targetModuleFolderPath);
            GameObject prefabContents = PrefabUtility.LoadPrefabContents(targetPrefabPath);

            if (prefabContents == null)
            {
                Debug.LogError("Failed to load prefab contents at " + targetPrefabPath);
                return;
            }

            ReplaceTemplateScreenViewScript(prefabContents, moduleName);
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

            Type newComponentType = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                newComponentType = assembly.GetType(className);
                if (newComponentType != null)
                {
                    break;
                }
            }

            if (newComponentType == null)
            {
                Debug.LogError($"Failed to find Type '{className}' even after scripts reload.");
                return;
            }

            ReplaceComponent(prefabContents, templateViewComponent, newComponentType);
        }

        private static void ReplaceComponent(GameObject gameObject, MonoBehaviour oldComponent, Type newComponentType)
        {
            var newComponent = gameObject.AddComponent(newComponentType) as MonoBehaviour;

            if (newComponent == null)
            {
                Debug.LogError("Failed to add new component.");
                return;
            }

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
