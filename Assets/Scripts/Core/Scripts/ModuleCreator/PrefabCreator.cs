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
            bool moduleCreationInProgress = EditorPrefs.
                GetBool(ModuleGenerator.ModuleCreationInProgressKey, false);
            if (moduleCreationInProgress)
            {
                string moduleName = EditorPrefs.GetString(ModuleGenerator.ModuleNameKey, "");
                string targetModuleFolderPath = EditorPrefs.GetString(ModuleGenerator.TargetModuleFolderPathKey, "");

                if (!string.IsNullOrEmpty(moduleName) && !string.IsNullOrEmpty(targetModuleFolderPath))
                {
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
            }
        }

        private static void CreatePrefabForModule(string moduleName, string targetModuleFolderPath)
        {
            string targetPrefabFolderPath = PathManager.CombinePaths(targetModuleFolderPath, "Views");
            ModuleGenerator.EnsureTargetFolderExists(targetPrefabFolderPath);

            string templateViewPrefabPath = PathManager.TemplateViewPrefabPath;
            string targetPrefabPath = PathManager.CombinePaths(targetPrefabFolderPath, $"{moduleName}View.prefab");

            AssetDatabase.CopyAsset(templateViewPrefabPath, targetPrefabPath);
            AssetDatabase.Refresh();

            GameObject prefabContents = PrefabUtility.LoadPrefabContents(targetPrefabPath);
            if (prefabContents != null)
            {
                var components = prefabContents.GetComponentsInChildren<Component>(true);
                foreach (var comp in components)
                {
                    if (comp == null)
                    {
                        string newScriptPath = PathManager.CombinePaths(targetModuleFolderPath, "Scripts",
                            $"{moduleName}ScreenView.cs");
                        MonoScript newScript = AssetDatabase.LoadAssetAtPath<MonoScript>(newScriptPath);

                        if (newScript != null)
                        {
                            Type newComponentType = newScript.GetClass();
                            if (newComponentType != null)
                            {
                                prefabContents.AddComponent(newComponentType);
                                RemoveMissingScripts(prefabContents);
                            }
                            else
                                Debug.LogError($"Failed to get Type from MonoScript at {newScriptPath}");
                        }
                        else
                            Debug.LogError($"Failed to load new script at {newScriptPath}");

                        break;
                    }
                }

                prefabContents.name = $"{moduleName}View";

                PrefabUtility.SaveAsPrefabAsset(prefabContents, targetPrefabPath);
                PrefabUtility.UnloadPrefabContents(prefabContents);
            }
            else
                Debug.LogError("Failed to load prefab contents at " + targetPrefabPath);
        }

        private static void RemoveMissingScripts(GameObject go)
        {
            var serializedObject = new SerializedObject(go);
            var prop = serializedObject.FindProperty("m_Component");

            int r = 0;
            for (int i = 0; i < prop.arraySize; i++)
            {
                var component = prop.GetArrayElementAtIndex(i - r);
                var obj = component.objectReferenceValue;
                if (obj == null)
                {
                    prop.DeleteArrayElementAtIndex(i - r);
                    r++;
                }
            }
            serializedObject.ApplyModifiedProperties();

            foreach (Transform child in go.transform)
                RemoveMissingScripts(child.gameObject);
        }
    }
}
