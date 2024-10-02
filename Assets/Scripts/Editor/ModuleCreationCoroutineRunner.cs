using System.Collections;
using UnityEditor;
using UnityEngine;
using Unity.EditorCoroutines.Editor;

namespace Editor
{
    public class ModuleCreationCoroutineRunner : ScriptableObject
    {
        public static void StartModuleCreationCoroutine(string moduleName, string targetModuleFolderPath)
        {
            var runner = CreateInstance<ModuleCreationCoroutineRunner>();
            // Используем EditorCoroutineUtility для запуска корутины
            EditorCoroutineUtility.StartCoroutine(runner.
                WaitForCompilationAndCreatePrefab(moduleName, targetModuleFolderPath), runner);
        }

        private IEnumerator WaitForCompilationAndCreatePrefab(string moduleName, string targetModuleFolderPath)
        {
            Debug.Log("Waiting for compilation to finish...");

            // Ждем, пока Unity компилирует скрипты
            while (EditorApplication.isCompiling)
            {
                Debug.Log("Still compiling...");
                yield return null;
            }

            Debug.Log("Compilation finished. Creating prefab.");

            // Создаем префаб
            PrefabCreator.CreatePrefabForModule(moduleName, targetModuleFolderPath);

            EditorUtility.DisplayDialog("Module Creation Finished",
                "Scripts compiled and prefab created successfully.", "OK");

            // Очищаем EditorPrefs
            EditorPrefs.DeleteKey(ModuleGenerator.ModuleCreationInProgressKey);
            EditorPrefs.DeleteKey(ModuleGenerator.ModuleNameKey);
            EditorPrefs.DeleteKey(ModuleGenerator.TargetModuleFolderPathKey);

            // Уничтожаем объект после завершения
            DestroyImmediate(this);
        }
    }
}