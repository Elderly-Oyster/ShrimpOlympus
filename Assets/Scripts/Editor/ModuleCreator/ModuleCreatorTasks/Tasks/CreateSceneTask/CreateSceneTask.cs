using System;
using System.Linq;
using System.Reflection;
using Core.Scripts.Views;
using Editor.ModuleCreator.ModuleCreatorTasks.Abstract;
using Editor.ModuleCreator.ModuleCreatorTasks.Tasks.AddScriptsTask;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Editor.ModuleCreator.ModuleCreatorTasks.Tasks.CreateSceneTask
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
            WaitForCompilation = true;
        }

        public override void Execute()
        {
            string sceneFolderPath = PathManager.CombinePaths(_targetModuleFolderPath, "Scene");
            ModuleGenerator.EnsureTargetFolderExists(sceneFolderPath);

            string scenePath = PathManager.CombinePaths(sceneFolderPath, $"{_moduleName}.unity");
            CreateNewScene(scenePath);

            GameObject canvas = CreateCanvas();
            GameObject viewInstance = InstantiateViewPrefab(canvas);
            GameObject installerObject = InstantiateInstaller();
            Camera camera = CreateModuleCamera();

            AssignInstallerFields(installerObject, viewInstance, canvas, camera);
            AssignScreensCanvasFields(canvas, camera);

            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), scenePath);
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }

        private void CreateNewScene(string scenePath)
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), scenePath);
        }

        private GameObject CreateCanvas()
        {
            GameObject canvas = new GameObject("Canvas");
            Canvas canvasComponent = canvas.AddComponent<Canvas>();
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();
            canvas.AddComponent<ScreensCanvas>();
            return canvas;
        }

        private GameObject InstantiateViewPrefab(GameObject canvas)
        {
            string viewPrefabPath = PathManager.CombinePaths(_targetModuleFolderPath,
                "Views", $"{_moduleName}View.prefab");
            GameObject viewPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(viewPrefabPath);
            if (viewPrefab != null)
            {
                GameObject viewInstance = PrefabUtility.InstantiatePrefab(viewPrefab) as GameObject;
                if (viewInstance != null)
                {
                    viewInstance.transform.SetParent(canvas.transform, false);
                    return viewInstance;
                }
                else
                {
                    Debug.LogError($"Failed to instantiate View prefab at {viewPrefabPath}");
                    return null;
                }
            }
            else
            {
                Debug.LogError($"View prefab not found at {viewPrefabPath}");
                return null;
            }
        }

        private GameObject InstantiateInstaller()
        {
            string installerScriptName = $"{_moduleName}Installer";
            Type installerType = FindType($"Modules.{GetFolderType(_targetModuleFolderPath)}." +
                                          $"{_moduleName}Screen.Scripts.{installerScriptName}");
            if (installerType != null)
            {
                GameObject installerObject = new GameObject(installerScriptName);
                installerObject.AddComponent(installerType);
                return installerObject;
            }
            else
            {
                Debug.LogError($"Installer type '{installerScriptName}' not found.");
                return null;
            }
        }

        private Camera CreateModuleCamera()
        {
            GameObject cameraObject = new GameObject("ModuleCamera");
            Camera cameraComponent = cameraObject.AddComponent<Camera>();
            cameraComponent.clearFlags = CameraClearFlags.Skybox;
            cameraComponent.cullingMask = LayerMask.GetMask("Default");
            return cameraComponent;
        }

        private void AssignInstallerFields(GameObject installerObject, GameObject viewInstance,
            GameObject canvas, Camera camera)
        {
            Component installerComponent = GetInstallerComponent(installerObject);
            Type installerType = installerComponent.GetType();

            string fieldPrefix = char.ToLower(_moduleName[0]) + _moduleName.Substring(1);
            string viewFieldName = $"{fieldPrefix}ScreenView";

            FieldInfo viewField = installerType.
                GetField(viewFieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo canvasField = installerType.
                GetField("screensCanvas", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo cameraField = installerType.
                GetField("mainCamera", BindingFlags.NonPublic | BindingFlags.Instance);

            if (viewField != null && viewInstance != null)
                viewField.SetValue(installerComponent, 
                    viewInstance.GetComponent($"{_moduleName}ScreenView"));
            else
                Debug.LogError("Field 'newModuleScreenView' not found or viewInstance is null.");

            if (canvasField != null)
                canvasField.SetValue(installerComponent, canvas.GetComponent<ScreensCanvas>());
            else
                Debug.LogError("Field 'screensCanvas' not found.");

            if (cameraField != null)
                cameraField.SetValue(installerComponent, camera);
            else
                Debug.LogError("Field 'mainCamera' not found.");
        }

        private Component GetInstallerComponent(GameObject installerObject)
        {
            if (installerObject == null) 
                return null;

            Component installerComponent = installerObject.GetComponent(installerObject.name);
            if (installerComponent == null)
            {
                Debug.LogError("Installer component is null.");
                return null;
            }

            return installerComponent;
        }

        private void AssignScreensCanvasFields(GameObject canvas, Camera camera)
        {
            ScreensCanvas screensCanvas = canvas.GetComponent<ScreensCanvas>();
            if (screensCanvas == null)
            {
                Debug.LogError("ScreensCanvas component not found on Canvas.");
                return;
            }

            Type screensCanvasType = screensCanvas.GetType();
            FieldInfo scalerField = screensCanvasType.
                GetField("canvasScaler", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo cameraField = screensCanvasType.
                GetField("uiCamera", BindingFlags.NonPublic | BindingFlags.Instance);

            if (scalerField != null)
                scalerField.SetValue(screensCanvas, canvas.GetComponent<CanvasScaler>());
            else
                Debug.LogError("Field 'canvasScaler' not found in ScreensCanvas.");

            if (cameraField != null)
                cameraField.SetValue(screensCanvas, camera);
            else
                Debug.LogError("Field 'uiCamera' not found in ScreensCanvas.");
        }

        private string GetFolderType(string path)
        {
            string[] pathParts = path.Split('/');
            int modulesIndex = Array.IndexOf(pathParts, "Modules");
            if (modulesIndex >= 0 && modulesIndex + 1 < pathParts.Length)
                return pathParts[modulesIndex + 1];
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