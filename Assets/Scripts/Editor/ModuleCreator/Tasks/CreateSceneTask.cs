// CreateSceneTask.cs
using System;
using System.Linq;
using System.Reflection;
using Editor.ModuleCreator.Tasks.Abstract;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using TMPro;
using UnityEngine.UI;

namespace Editor.ModuleCreator.Tasks
{
    [Serializable]
    public class CreateSceneTask : Task
    {
        [JsonProperty] private string _moduleName;
        [JsonProperty] private string _targetModuleFolderPath;

        public CreateSceneTask(string moduleName, string targetModuleFolderPath)
        {
            _moduleName = moduleName;
            _targetModuleFolderPath = targetModuleFolderPath;
            WaitForCompilation = false;
        }

        public override void Execute()
        {
            string sceneFolderPath = PathManager.CombinePaths(_targetModuleFolderPath, "Scene");
            ModuleGenerator.EnsureTargetFolderExists(sceneFolderPath);

            string scenePath = PathManager.CombinePaths(sceneFolderPath, $"{_moduleName}.unity");
            CreateNewScene(scenePath);

            GameObject canvas = CreateCanvas();
            InstantiateViewPrefab(canvas);
            InstantiateInstaller(canvas);
            CreateModuleCamera();

            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), scenePath);
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }

        private void CreateNewScene(string scenePath)
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), scenePath);
        }

        private GameObject CreateCanvas()
        {
            GameObject canvas = new GameObject("Canvas");
            Canvas canvasComponent = canvas.AddComponent<Canvas>();
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private void InstantiateViewPrefab(GameObject canvas)
        {
            string viewPrefabPath = PathManager.CombinePaths(_targetModuleFolderPath, "Views", $"{_moduleName}View.prefab");
            GameObject viewPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(viewPrefabPath);
            if (viewPrefab != null)
            {
                GameObject viewInstance = PrefabUtility.InstantiatePrefab(viewPrefab) as GameObject;
                if (viewInstance != null)
                {
                    viewInstance.transform.SetParent(canvas.transform, false);
                }
                else
                {
                    Debug.LogError($"Failed to instantiate View prefab at {viewPrefabPath}");
                }
            }
            else
            {
                Debug.LogError($"View prefab not found at {viewPrefabPath}");
            }
        }

        private void InstantiateInstaller(GameObject canvas)
        {
            string installerScriptName = $"{_moduleName}Installer";
            Type installerType = FindType($"Modules.{GetFolderType(_targetModuleFolderPath)}.{_moduleName}Screen.Scripts.{installerScriptName}");
            if (installerType != null)
            {
                GameObject installerObject = new GameObject(installerScriptName);
                installerObject.transform.SetParent(canvas.transform, false);
                installerObject.AddComponent(installerType);
            }
            else
            {
                Debug.LogError($"Installer type '{installerScriptName}' not found.");
            }
        }

        private void CreateModuleCamera()
        {
            GameObject cameraObject = new GameObject("ModuleCamera");
            Camera cameraComponent = cameraObject.AddComponent<Camera>();
            cameraComponent.clearFlags = CameraClearFlags.Skybox;
            cameraComponent.cullingMask = LayerMask.GetMask("Default");
        }

        private string GetFolderType(string path)
        {
            string[] pathParts = path.Split('/');
            int modulesIndex = Array.IndexOf(pathParts, "Modules");
            if (modulesIndex >= 0 && modulesIndex + 1 < pathParts.Length)
            {
                return pathParts[modulesIndex + 1];
            }
            Debug.LogError("Folder type not found in path: " + path);
            return "";
        }

        private Type FindType(string fullName)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.FullName == fullName);
        }
    }
}