using System;
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
            Debug.Log($"Executing CreateSceneTask for module: {_moduleName}");
            string sceneFolderPath = PathManager.CombinePaths(_targetModuleFolderPath, "Scene");
            ModuleGenerator.EnsureTargetFolderExists(sceneFolderPath);

            string scenePath = PathManager.CombinePaths(sceneFolderPath, $"{_moduleName}.unity");
            CreateNewScene(scenePath);

            GameObject canvas = CreateCanvas();
            if (canvas == null) { Debug.LogError("Failed to create Canvas."); return; }

            string viewPrefabPath = PathManager.CombinePaths(_targetModuleFolderPath, "Views", $"{_moduleName}View.prefab");
            GameObject viewInstance = InstantiateViewPrefab(viewPrefabPath, canvas);
            if (viewInstance == null) { Debug.LogError("Failed to instantiate View prefab."); return; }

            GameObject installerObject = InstantiateInstaller();
            if (installerObject == null) { Debug.LogError("Failed to instantiate Installer."); return; }

            Camera camera = CreateModuleCamera();
            if (camera == null) { Debug.LogError("Failed to create Module Camera."); return; }

            AssignInstallerFields(installerObject, viewInstance, canvas, camera);
            AssignScreensCanvasFields(canvas, camera);

            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), scenePath);
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }

        private void CreateNewScene(string scenePath)
        {
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            if (newScene.IsValid())
            {
                EditorSceneManager.SaveScene(newScene, scenePath);
                EditorSceneManager.SetActiveScene(newScene);
            }
            else
                Debug.LogError("Failed to create a new scene.");
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

        private GameObject InstantiateViewPrefab(string prefabPath, GameObject parent)
        {
            GameObject viewPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (viewPrefab == null)
            {
                Debug.LogError($"View prefab not found at {prefabPath}");
                return null;
            }

            GameObject viewInstance = PrefabUtility.InstantiatePrefab(viewPrefab) as GameObject;
            if (viewInstance == null)
            {
                Debug.LogError($"Failed to instantiate View prefab at {prefabPath}");
                return null;
            }

            viewInstance.transform.SetParent(parent.transform, false);
            return viewInstance;
        }

        private GameObject InstantiateInstaller()
        {
            string installerScriptName = $"{_moduleName}Installer";
            string folderType = PathManager.GetFolderType(_targetModuleFolderPath);
            string installerFullName = $"Modules.{folderType}.{_moduleName}Screen.Scripts.{installerScriptName}";
            Type installerType = ReflectionHelper.FindType(installerFullName);

            if (installerType != null)
                return new GameObject(installerScriptName).AddComponent(installerType).gameObject;
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
            Component installerComponent = ReflectionHelper.GetComponentByName(installerObject, installerObject.name);
            if (installerComponent == null) return;

            string fieldPrefix = char.ToLower(_moduleName[0]) + _moduleName.Substring(1);
            string viewFieldName = $"{fieldPrefix}ScreenView";

            ReflectionHelper.SetPrivateField(installerComponent, viewFieldName,
                viewInstance?.GetComponent($"{_moduleName}ScreenView"));
            ReflectionHelper.SetPrivateField(installerComponent, "screensCanvas",
                canvas.GetComponent<ScreensCanvas>());
            ReflectionHelper.SetPrivateField(installerComponent, "mainCamera", camera);
        }

        private void AssignScreensCanvasFields(GameObject canvas, Camera camera)
        {
            ScreensCanvas screensCanvas = canvas.GetComponent<ScreensCanvas>();
            if (screensCanvas == null)
            {
                Debug.LogError("ScreensCanvas component not found on Canvas.");
                return;
            }

            ReflectionHelper.SetPrivateField(screensCanvas, "canvasScaler", canvas.GetComponent<CanvasScaler>());
            ReflectionHelper.SetPrivateField(screensCanvas, "uiCamera", camera);
        }
    }
}
