using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Editor.ModuleCreator;
using Editor.ModuleCreator.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Editor.ModuleCreator
{
    public class ModuleCreatorWindowTests
    {
        private const string ModuleName = "NewModule";
        private const string TrackingFilePath = "Assets/Scripts/Editor/ModuleCreator/CreatedModules.json";

        [SetUp]
        public void Setup()
        {
            if (File.Exists(TrackingFilePath))
            {
                File.Delete(TrackingFilePath);
                const string metaPath = TrackingFilePath + ".meta";
                if (File.Exists(metaPath))
                    File.Delete(metaPath);
            }
            AssetDatabase.Refresh();
        }

        [TearDown]
        public void Teardown()
        {
            if (File.Exists(TrackingFilePath))
            {
                File.Delete(TrackingFilePath);
                const string metaPath = TrackingFilePath + ".meta";
                if (File.Exists(metaPath))
                    File.Delete(metaPath);
            }
            AssetDatabase.Refresh();
        }

        [UnityTest]
        public IEnumerator CreateModule_ShouldCreateModuleFolder() => UniTask.ToCoroutine(async () =>
        {
            // arrange 
            var moduleCreatorWindow = ScriptableObject.CreateInstance<ModuleCreatorWindow>();
            moduleCreatorWindow.ShowUtility();
            moduleCreatorWindow.CreateModule();
            
            // act
            await TaskQueue.UniTaskCompletionSource.Task;

            // assert
            //AssetDatabase.Refresh();

            Assert.IsTrue(File.Exists(TrackingFilePath), "Tracking file was not created.");

            string json = await File.ReadAllTextAsync(TrackingFilePath);
            List<string> createdModules = JsonConvert.DeserializeObject<List<string>>(json);
            Assert.IsNotNull(createdModules, "CreatedModules list is null.");
            Assert.IsTrue(createdModules.Count > 0, "CreatedModules list is empty.");

            string targetModuleFolderPath = createdModules.Find(path => path.Contains(ModuleName));
            Assert.IsFalse(string.IsNullOrEmpty(targetModuleFolderPath),
                "Module path not found in tracking file.");

            Debug.Log($"Created module path: {targetModuleFolderPath}");

            Assert.IsTrue(AssetDatabase.IsValidFolder(targetModuleFolderPath),
                "Module folder was not created.");
        });
    }
}