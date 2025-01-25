using CodeBase.Core.Root;
using CodeBase.Editor.ModuleCreator.Tasks.CreateSceneTask;
using CodeBase.Implementation.Root;
using CodeBase.Implementation.UI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Tests.Editor.ModuleCreator
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
            ScreenCanvas screenCanvas = canvas.GetComponent<ScreenCanvas>();
            Assert.IsNotNull(screenCanvas);
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