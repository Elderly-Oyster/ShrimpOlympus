using System;
using System.Reflection;
using Editor.ModuleCreator.ModuleCreatorTasks.Tasks.AddScriptsTask;
using Modules.Template.TemplateScreen.Scripts;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Editor.ModuleCreator.ModuleCreatorTasks.Tasks.AddPrefabTask
{
    public static class PrefabCreator
    {
        public static void CreatePrefabForModule(string moduleName, string targetModuleFolderPath)
        {
            string targetPrefabPath = CopyTemplatePrefab(moduleName, targetModuleFolderPath);
            if (IsInvalidPath(targetPrefabPath))
                return;

            GameObject prefabContents = LoadPrefab(targetPrefabPath);
            if (prefabContents == null)
                return;

            ReplaceTemplateScreenViewScript(prefabContents, moduleName, targetModuleFolderPath);

            MonoScript newMonoScript = GetMonoScript(moduleName, targetModuleFolderPath);
            if (newMonoScript == null)
            {
                PrefabUtility.UnloadPrefabContents(prefabContents);
                return;
            }

            Type newViewType = GetViewType(newMonoScript);
            if (newViewType == null)
            {
                PrefabUtility.UnloadPrefabContents(prefabContents);
                return;
            }

            Component newViewComponent = GetViewComponent(prefabContents, newViewType);
            if (newViewComponent != null)
            {
                AssignTemplateScreenTitle(prefabContents, moduleName, newViewComponent, newViewType);
                InvokeSetTitle(newViewComponent, moduleName, newViewType);
                LogTitleSet(moduleName, newViewType.Name);
            }
            else
                LogComponentNotFound(newViewType.Name);

            SaveAndUnloadPrefab(prefabContents, targetPrefabPath, moduleName);
            LogPrefabCreated(moduleName);
        }

        private static string CopyTemplatePrefab(string moduleName, string targetModuleFolderPath)
        {
            string targetPrefabFolderPath = PathManager.CombinePaths(targetModuleFolderPath, "Views");
            ModuleGenerator.EnsureTargetFolderExists(targetPrefabFolderPath);

            string templateViewPrefabPath = PathManager.TemplateViewPrefabPath;
            string targetPrefabPath = PathManager.CombinePaths(targetPrefabFolderPath, $"{moduleName}View.prefab");
            
            bool copyResult = AssetDatabase.CopyAsset(templateViewPrefabPath, targetPrefabPath);
            if (!copyResult)
            {
                Debug.LogError($"Failed to copy prefab from '{templateViewPrefabPath}' to '{targetPrefabPath}'.");
                return null;
            }
            AssetDatabase.ImportAsset(targetPrefabPath, ImportAssetOptions.ForceUpdate);

            return targetPrefabPath;
        }

        private static bool IsInvalidPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("Target prefab path is null or empty.");
                return true;
            }
            return false;
        }

        private static GameObject LoadPrefab(string prefabPath)
        {
            GameObject prefab = PrefabUtility.LoadPrefabContents(prefabPath);
            if (prefab == null) 
                Debug.LogError($"Failed to load prefab contents at {prefabPath}");
            return prefab;
        }

        private static void ReplaceTemplateScreenViewScript(GameObject prefabContents, string moduleName, string targetModuleFolderPath)
        {
            TemplateScreenView templateViewComponent = prefabContents.GetComponent<TemplateScreenView>();
            if (templateViewComponent == null)
            {
                Debug.LogError("TemplateScreenView component not found in prefab.");
                return;
            }

            string folderType = GetFolderType(targetModuleFolderPath);
            if (string.IsNullOrEmpty(folderType)) return;

            string fullClassName = GetFullClassName(moduleName, folderType);
            MonoScript newMonoScript = GetMonoScript(moduleName, targetModuleFolderPath);
            if (newMonoScript == null)
            {
                Debug.LogError($"MonoScript for class '{fullClassName}' not found. Ensure the script is compiled.");
                return;
            }

            ReplaceScriptProperty(templateViewComponent, newMonoScript, fullClassName);
        }

        private static string GetFolderType(string path)
        {
            string[] pathParts = path.Split('/');
            int modulesIndex = Array.IndexOf(pathParts, "Modules");
            if (modulesIndex >= 0 && modulesIndex + 1 < pathParts.Length)
            {
                string folderType = pathParts[modulesIndex + 1];
                Debug.Log($"Determined folder type: {folderType}");
                return folderType;
            }
            Debug.LogError("Folder type not found in path: " + path);
            return "";
        }

        private static string GetFullClassName(string moduleName, string folderType)
        {
            string namespaceName = $"Modules.{folderType}.{moduleName}Screen.Scripts";
            string className = $"{moduleName}ScreenView";
            return $"{namespaceName}.{className}";
        }

        private static MonoScript GetMonoScript(string moduleName, string targetModuleFolderPath)
        {
            string folderType = GetFolderType(targetModuleFolderPath);
            string fullClassName = GetFullClassName(moduleName, folderType);

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

        private static void ReplaceScriptProperty(TemplateScreenView templateViewComponent, MonoScript newMonoScript, string fullClassName)
        {
            SerializedObject serializedObject = new SerializedObject(templateViewComponent);
            SerializedProperty scriptProperty = serializedObject.FindProperty("m_Script");

            if (scriptProperty != null)
            {
                scriptProperty.objectReferenceValue = newMonoScript;
                serializedObject.ApplyModifiedProperties();
                Debug.Log($"Replaced script on TemplateScreenView with {fullClassName}.");
            }
            else
                Debug.LogError("Failed to find 'm_Script' property.");
        }

        private static Type GetViewType(MonoScript monoScript) => monoScript.GetClass();

        private static Component GetViewComponent(GameObject prefabContents, Type viewType)
        {
            Component component = prefabContents.GetComponent(viewType);
            if (component == null) 
                Debug.LogError($"{viewType.Name} component not found in prefab.");
            return component;
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

            FieldInfo fieldInfo = viewType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(newViewComponent, titleText);
                Debug.Log($"Assigned '{fieldName}' to TMP_Text component.");
            }
            else
                Debug.LogError($"Field '{fieldName}' not found in '{viewType.Name}'.");
        }

        private static void InvokeSetTitle(Component newViewComponent, string moduleName, Type viewType)
        {
            MethodInfo setTitleMethod = viewType.GetMethod("SetTitle", BindingFlags.Public | BindingFlags.Instance);
            if (setTitleMethod != null)
                setTitleMethod.Invoke(newViewComponent, new object[] { moduleName });
            else
                Debug.LogError($"SetTitle method not found in '{viewType.Name}'.");
        }

        private static void SaveAndUnloadPrefab(GameObject prefabContents, string prefabPath, string moduleName)
        {
            prefabContents.name = $"{moduleName}View";
            PrefabUtility.SaveAsPrefabAsset(prefabContents, prefabPath);
            PrefabUtility.UnloadPrefabContents(prefabContents);
        }

        private static void LogTitleSet(string moduleName, string viewTypeName) => 
            Debug.Log($"Set title to '{moduleName}' in {viewTypeName}.");

        private static void LogComponentNotFound(string viewTypeName) => 
            Debug.LogError($"{viewTypeName} component not found in prefab.");

        private static void LogPrefabCreated(string moduleName) => 
            Debug.Log($"Prefab for module {moduleName} created successfully.");
    }
}
