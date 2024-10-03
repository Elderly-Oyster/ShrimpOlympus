using System;
using Editor.Tasks.Abstract;
using UnityEngine;

namespace Editor.Tasks
{
    [Serializable]
    public class AddPrefabTask : Task
    {
        [SerializeField] private string _moduleName;
        [SerializeField] private string _targetModuleFolderPath;

        public AddPrefabTask(string moduleName, string targetModuleFolderPath)
        {
            _moduleName = moduleName;
            _targetModuleFolderPath = targetModuleFolderPath;
            WaitForCompilation = true;
        }

        public override void Execute()
        {
            Debug.Log("Executing AddPrefabTask for module: " + _moduleName);
            PrefabCreator.CreatePrefabForModule(_moduleName, _targetModuleFolderPath);
        }
    }
}