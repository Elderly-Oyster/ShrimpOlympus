using Core.Scripts.Views;
using Editor.ModuleCreator.Tasks.CreateSceneTask;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Tests.Editor.ModuleCreator
{
    public class GameObjectFactoryTests
    {
        [Test]
        public void CreateCanvas_ShouldReturnCanvasWithRequiredComponents()
        {
            GameObject canvas = GameObjectFactory.CreateCanvas();
            Assert.IsNotNull(canvas);
            Canvas canvasComponent = canvas.GetComponent<Canvas>();
            Assert.IsNotNull(canvasComponent);
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            Assert.IsNotNull(scaler);
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            Assert.IsNotNull(raycaster);
            ScreensCanvas screensCanvas = canvas.GetComponent<ScreensCanvas>();
            Assert.IsNotNull(screensCanvas);
            Object.DestroyImmediate(canvas);
        }

        [Test]
        public void CreateModuleCamera_ShouldReturnCameraWithCorrectSettings()
        {
            Camera camera = GameObjectFactory.CreateModuleCamera();
            Assert.IsNotNull(camera);
            Assert.AreEqual(CameraClearFlags.Skybox, camera.clearFlags);
            Assert.AreEqual(LayerMask.GetMask("Default"), camera.cullingMask);
            Object.DestroyImmediate(camera.gameObject);
        }
    }
}