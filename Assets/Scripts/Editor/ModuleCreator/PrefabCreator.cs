using System;
using System.Linq;
using System.Reflection;
using Modules.Template.TemplateScreen.Scripts;
using UnityEditor;
using UnityEngine;
using TMPro;

namespace Editor.ModuleCreator
{
    public static class PrefabCreator
    {
        public static void CreatePrefabForModule(string moduleName, string targetModuleFolderPath)
        {
            Debug.Log($"CreatePrefabForModule called with moduleName: {moduleName}, targetModuleFolderPath: {targetModuleFolderPath}");

            string targetPrefabPath = CopyTemplatePrefab(moduleName, targetModuleFolderPath);
            if (string.IsNullOrEmpty(targetPrefabPath))
            {
                Debug.LogError("Target prefab path is null or empty.");
                return;
            }

            GameObject prefabContents = PrefabUtility.LoadPrefabContents(targetPrefabPath);

            if (prefabContents == null)
            {
                Debug.LogError($"Failed to load prefab contents at {targetPrefabPath}");
                return;
            }

            ReplaceTemplateScreenViewScript(prefabContents, moduleName, targetModuleFolderPath);

            MonoScript newMonoScript = GetMonoScript(moduleName, targetModuleFolderPath);
            if (newMonoScript == null)
            {
                Debug.LogError("New MonoScript is null, cannot proceed.");
                PrefabUtility.UnloadPrefabContents(prefabContents);
                return;
            }

            Type newViewType = newMonoScript.GetClass();
            if (newViewType == null)
            {
                Debug.LogError("Failed to get Type from new MonoScript.");
                PrefabUtility.UnloadPrefabContents(prefabContents);
                return;
            }

            Component newViewComponent = prefabContents.GetComponent(newViewType);
            if (newViewComponent != null)
            {
                AssignTemplateScreenTitle(prefabContents, moduleName, newViewComponent, newViewType);
                InvokeSetTitle(newViewComponent, moduleName, newViewType);
                Debug.Log($"Set title to '{moduleName}' in {newViewType.Name}.");
            }
            else
            {
                Debug.LogError($"{newViewType.Name} component not found in prefab.");
            }

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

        private static void ReplaceTemplateScreenViewScript(GameObject prefabContents, string moduleName, string targetModuleFolderPath)
        {
            var templateViewComponent = prefabContents.GetComponent<TemplateScreenView>();
            if (templateViewComponent == null)
            {
                Debug.LogError("TemplateScreenView component not found in prefab.");
                return;
            }

            string folderType = GetFolderTypeFromPath(targetModuleFolderPath);
            if (string.IsNullOrEmpty(folderType))
            {
                Debug.LogError("Folder type could not be determined from the target module folder path.");
                return;
            }

            string namespaceName = $"Modules.{folderType}.{moduleName}Screen.Scripts";
            string className = $"{moduleName}ScreenView";
            string fullClassName = $"{namespaceName}.{className}";

            MonoScript newMonoScript = GetMonoScript(moduleName, targetModuleFolderPath);
            if (newMonoScript == null)
            {
                Debug.LogError($"MonoScript for class '{fullClassName}' not found. Ensure the script is compiled.");
                return;
            }

            SerializedObject serializedObject = new SerializedObject(templateViewComponent);
            SerializedProperty scriptProperty = serializedObject.FindProperty("m_Script");

            if (scriptProperty != null)
            {
                scriptProperty.objectReferenceValue = newMonoScript;
                serializedObject.ApplyModifiedProperties();
                Debug.Log($"Replaced script on TemplateScreenView with {fullClassName}.");
            }
            else
            {
                Debug.LogError("Failed to find 'm_Script' property.");
            }
        }

        private static MonoScript GetMonoScript(string moduleName, string targetModuleFolderPath)
        {
            string folderType = GetFolderTypeFromPath(targetModuleFolderPath);
            string namespaceName = $"Modules.{folderType}.{moduleName}Screen.Scripts";
            string className = $"{moduleName}ScreenView";
            string fullClassName = $"{namespaceName}.{className}";

            // Ищем MonoScript по полному имени класса
            string[] guids = AssetDatabase.FindAssets("t:MonoScript");
            Debug.Log($"Searching for MonoScript with class name: {fullClassName}");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                if (monoScript != null && monoScript.GetClass() != null)
                {
                    if (monoScript.GetClass().FullName == fullClassName)
                    {
                        Debug.Log($"Found MonoScript: {monoScript.GetClass().FullName} at {path}");
                        return monoScript;
                    }
                }
            }
            Debug.LogError($"MonoScript for class '{fullClassName}' not found.");
            return null;
        }

        private static void AssignTemplateScreenTitle(GameObject prefabContents, string moduleName, Component newViewComponent, Type viewType)
        {
            string fieldName = $"{char.ToLower(moduleName[0])}{moduleName.Substring(1)}ScreenTitle";


            Transform titleTransform = prefabContents.transform.Find("Title");
            if (titleTransform == null)
            {
                Debug.LogError("Title GameObject not found in prefab.");
                return;
            }

            TMP_Text titleText = titleTransform.GetComponent<TMP_Text>();
            if (titleText == null)
            {
                Debug.LogError("TMP_Text component not found on Title GameObject.");
                return;
            }

            // Используем рефлексию для назначения поля
            FieldInfo fieldInfo = viewType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(newViewComponent, titleText);
                Debug.Log($"Assigned '{fieldName}' to TMP_Text component.");
            }
            else
            {
                Debug.LogError($"Field '{fieldName}' not found in '{viewType.Name}'.");
            }
        }

        private static void InvokeSetTitle(Component newViewComponent, string moduleName, Type viewType)
        {
            // Находим метод SetTitle
            MethodInfo setTitleMethod = viewType.GetMethod("SetTitle", BindingFlags.Public | BindingFlags.Instance);
            if (setTitleMethod != null)
            {
                setTitleMethod.Invoke(newViewComponent, new object[] { moduleName });
            }
            else
            {
                Debug.LogError($"SetTitle method not found in '{viewType.Name}'.");
            }
        }

        private static string GetFolderTypeFromPath(string targetModuleFolderPath)
        {
            string[] pathParts = targetModuleFolderPath.Split('/');
            int modulesIndex = Array.IndexOf(pathParts, "Modules");
            if (modulesIndex >= 0 && modulesIndex + 1 < pathParts.Length)
            {
                string folderType = pathParts[modulesIndex + 1];
                Debug.Log($"Determined folder type: {folderType}");
                return folderType;
            }
            else
            {
                Debug.LogError("Folder type not found in path: " + targetModuleFolderPath);
                return "";
            }
        }

        private static void SaveAndUnloadPrefab(GameObject prefabContents, string prefabPath, string moduleName)
        {
            prefabContents.name = $"{moduleName}View";
            PrefabUtility.SaveAsPrefabAsset(prefabContents, prefabPath);
            PrefabUtility.UnloadPrefabContents(prefabContents);
        }
    }
}