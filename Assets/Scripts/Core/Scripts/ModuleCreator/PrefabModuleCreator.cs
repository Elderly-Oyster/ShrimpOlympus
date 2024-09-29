using System.IO;
using UnityEditor;
using UnityEngine;

namespace Core.Scripts.ModuleCreator
{
    public class PrefabModuleCreator
    {
        private readonly string _templateViewsFolderPath;

        public PrefabModuleCreator(string templateModuleFolderPath) => 
            _templateViewsFolderPath = Path.Combine(templateModuleFolderPath, "Views");

        public void CreatePrefab(string targetFolderPath, string moduleName)
        {
            string templatePrefabPath = Path.Combine(_templateViewsFolderPath, "TemplateView.prefab");

            if (!File.Exists(templatePrefabPath))
            {
                Debug.LogError($"Template prefab not found at {templatePrefabPath}");
                return;
            }

            string targetViewsFolderPath = Path.Combine(targetFolderPath, "Views");
            BaseModuleCreator.EnsureTargetFolderExists(targetViewsFolderPath);

            string targetPrefabPath = Path.Combine(targetViewsFolderPath, $"{moduleName}View.prefab");

            AssetDatabase.CopyAsset(templatePrefabPath, targetPrefabPath);

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(targetPrefabPath);
            if (prefab == null)
            {
                Debug.LogError($"Failed to load prefab at {targetPrefabPath}");
                return;
            }

            AdjustPrefab(prefab, moduleName);

            EditorUtility.SetDirty(prefab);
            AssetDatabase.SaveAssets();
        }

        private static void AdjustPrefab(GameObject prefab, string moduleName)
        {
            string newScriptName = $"{moduleName}ScreenView";
            MonoScript newScript = FindScriptAsset(newScriptName);

            if (newScript == null)
            {
                Debug.LogError($"Could not find script {newScriptName} in project.");
                return;
            }

            MonoBehaviour[] components = prefab.GetComponents<MonoBehaviour>();
            foreach (var component in components)
            {
                MonoScript componentScript = MonoScript.FromMonoBehaviour(component);
                if (componentScript != null && componentScript.name == "TemplateScreenView")
                {
                    SerializedObject serializedObject = new SerializedObject(component);
                    serializedObject.FindProperty("m_Script").objectReferenceValue = newScript;
                    serializedObject.ApplyModifiedProperties();
                    break;
                }
            }

            prefab.name = $"{moduleName}View";
        }

        private static MonoScript FindScriptAsset(string scriptName)
        {
            string[] guids = AssetDatabase.FindAssets($"{scriptName} t:MonoScript");
            if (guids.Length == 0)
                return null;

            string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
        }
    }
}
