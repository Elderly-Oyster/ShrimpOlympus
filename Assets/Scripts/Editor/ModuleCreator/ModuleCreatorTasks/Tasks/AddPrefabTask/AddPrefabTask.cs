using System;
using Editor.ModuleCreator.ModuleCreatorTasks.Abstract;
using Newtonsoft.Json;
using UnityEngine;

namespace Editor.ModuleCreator.ModuleCreatorTasks.Tasks.AddPrefabTask
{
    [Serializable]
    public class AddPrefabTask : Task
    {
        [JsonProperty] private string _moduleName;
        [JsonProperty] private string _targetModuleFolderPath;

        public AddPrefabTask(string moduleName, string targetModuleFolderPath)
        {
            _moduleName = moduleName;
            _targetModuleFolderPath = targetModuleFolderPath;
            WaitForCompilation = true; 
        }

        public override void Execute()
        {
            Debug.Log($"Executing AddPrefabTask for module: {_moduleName}");
            Debug.Log($"Target Module Folder Path: {_targetModuleFolderPath}");
            PrefabCreator.CreatePrefabForModule(_moduleName, _targetModuleFolderPath);
        }
    }
}