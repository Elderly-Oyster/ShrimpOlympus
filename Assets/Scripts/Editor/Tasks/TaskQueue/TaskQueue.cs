using System;
using System.Collections.Generic;
using System.Linq;
using Editor.Tasks.Abstract;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Editor.Tasks.TaskQueue
{
    [InitializeOnLoad]
    public static class TaskQueue
    {
        private static readonly Queue<Task> Tasks = new Queue<Task>();
        private static bool _isExecuting = false;

        static TaskQueue()
        {
            LoadState();
            EditorApplication.update += OnEditorUpdate;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        public static void EnqueueTask(Task task)
        {
            Debug.Log("EnqueueTask: " + task.GetType().Name);
            Tasks.Enqueue(task);
            SaveState();
            ExecuteNextTask();
        }

        private static void ExecuteNextTask()
        {
            while (true)
            {
                if (_isExecuting || Tasks.Count == 0)
                {
                    Debug.Log("No tasks to execute or already executing.");
                    return;
                }

                _isExecuting = true;
                var task = Tasks.Peek();
                Debug.Log("Executing task: " + task.GetType().Name);
                task.Execute();

                if (task.WaitForCompilation)
                {
                    Debug.Log("Waiting for compilation...");
                    EditorApplication.update += WaitForCompilation;
                }
                else
                {
                    Debug.Log("Task does not require compilation wait.");
                    Tasks.Dequeue();
                    _isExecuting = false;
                    SaveState();
                    continue;
                }

                break;
            }
        }

        private static void WaitForCompilation()
        {
            if (!EditorApplication.isCompiling)
            {
                Debug.Log("Compilation finished.");
                EditorApplication.update -= WaitForCompilation;
                Tasks.Dequeue();
                _isExecuting = false;
                SaveState();
                ExecuteNextTask();
            }
        }

        private static void OnEditorUpdate()
        {
            if (!_isExecuting && Tasks.Count > 0)
                ExecuteNextTask();
        }

        private static void OnAfterAssemblyReload()
        {
            Debug.Log("Assembly reloaded.");
            LoadState();
        }

        private static void SaveState()
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            string tasksJson = JsonConvert.SerializeObject(Tasks.ToList(), settings);
            Debug.Log("Saving tasks: " + tasksJson);
            SessionState.SetString("TaskQueueState", tasksJson);
        }

        private static void LoadState()
        {
            // Не удаляем сохраненное состояние
            string tasksJson = SessionState.GetString("TaskQueueState", "");
            Debug.Log("Loading tasks: " + tasksJson);
            if (!string.IsNullOrEmpty(tasksJson))
            {
                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                try
                {
                    var tasksList = JsonConvert.DeserializeObject<List<Task>>(tasksJson, settings);
                    Tasks.Clear();
                    foreach (var task in tasksList)
                    {
                        Debug.Log("Enqueuing loaded task: " + task.GetType().Name);
                        Tasks.Enqueue(task);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error deserializing tasks: " + ex.Message);
                }
            }
            else
            {
                Debug.Log("No tasks found in SessionState.");
            }
        }
    }
}
