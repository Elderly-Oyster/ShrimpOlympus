using System;
using UnityEditor;
using UnityEngine;

namespace Core.Scripts.ModuleCreator
{
    [InitializeOnLoad]
    public static class PrefabCreator
    {
        static PrefabCreator() => EditorApplication.update += OnEditorUpdate;

        private static void OnEditorUpdate()
        {
            if (!EditorPrefs.GetBool(ModuleGenerator.ModuleCreationInProgressKey, false))
                return;

            string moduleName = EditorPrefs.GetString(ModuleGenerator.ModuleNameKey, "");
            string targetModuleFolderPath = EditorPrefs.GetString(ModuleGenerator.TargetModuleFolderPathKey, "");

            if (string.IsNullOrEmpty(moduleName) || string.IsNullOrEmpty(targetModuleFolderPath)) return;

            EditorApplication.delayCall += () =>
            {
                CreatePrefabForModule(moduleName, targetModuleFolderPath);

                EditorPrefs.DeleteKey(ModuleGenerator.ModuleCreationInProgressKey);
                EditorPrefs.DeleteKey(ModuleGenerator.ModuleNameKey);
                EditorPrefs.DeleteKey(ModuleGenerator.TargetModuleFolderPathKey);

                EditorUtility.DisplayDialog("Module Creation Finished",
                    "Scripts compiled and prefab created successfully.", "OK");

                EditorApplication.update -= OnEditorUpdate;
            };
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

            AddComponentToPrefab(prefabContents, moduleName, targetModuleFolderPath);
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

        private static void AddComponentToPrefab(GameObject prefabContents,
            string moduleName, string targetModuleFolderPath)
        {
            var components = prefabContents.GetComponentsInChildren<Component>(true);
            foreach (var comp in components)
            {
                if (comp != null) continue;

                string newScriptPath = PathManager.
                    CombinePaths(targetModuleFolderPath, "Scripts", $"{moduleName}ScreenView.cs");
                MonoScript newScript = AssetDatabase.LoadAssetAtPath<MonoScript>(newScriptPath);

                if (newScript == null)
                {
                    Debug.LogError($"Failed to load new script at {newScriptPath}");
                    break;
                }

                Type newComponentType = newScript.GetClass();
                if (newComponentType == null)
                {
                    Debug.LogError($"Failed to get Type from MonoScript at {newScriptPath}");
                    break;
                }

                prefabContents.AddComponent(newComponentType);
                RemoveMissingScripts(prefabContents);
                break;
            }
        }

        private static void SaveAndUnloadPrefab(GameObject prefabContents, string prefabPath, string moduleName)
        {
            prefabContents.name = $"{moduleName}View";
            PrefabUtility.SaveAsPrefabAsset(prefabContents, prefabPath);
            PrefabUtility.UnloadPrefabContents(prefabContents);
        }

        private static void RemoveMissingScripts(GameObject go)
        {
            var serializedObject = new SerializedObject(go);
            var prop = serializedObject.FindProperty("m_Component");

            int r = 0;
            for (int i = 0; i < prop.arraySize; i++)
            {
                var component = prop.GetArrayElementAtIndex(i - r);
                if (component.objectReferenceValue != null) continue;

                prop.DeleteArrayElementAtIndex(i - r);
                r++;
            }
            serializedObject.ApplyModifiedProperties();

            foreach (Transform child in go.transform)
                RemoveMissingScripts(child.gameObject);
        }
    }
}