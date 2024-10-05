using System;
using System.Collections.Generic;
using System.Linq;
using Editor.ModuleCreator.ModuleCreatorTasks.Abstract;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Editor.ModuleCreator.ModuleCreatorTasks.TaskQueue
{
    [InitializeOnLoad]
    public static class TaskQueue
    {
        private static readonly Queue<Task> Tasks = new();
        private static bool _isExecuting;

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
            if (_isExecuting || Tasks.Count == 0)
            {
                Debug.Log("No tasks to execute or already executing.");
                return;
            }

            _isExecuting = true;
            var task = Tasks.Peek();
            Debug.Log("Executing task: " + task.GetType().Name);
            task.Execute();
            HandleTaskCompletion(task);
        }

        private static void HandleTaskCompletion(Task task)
        {
            if (task.WaitForCompilation)
            {
                if (EditorApplication.isCompiling)
                {
                    Debug.Log("Waiting for compilation...");
                    EditorApplication.update += WaitForCompilation;
                }
                else
                {
                    Debug.Log("No compilation detected after task execution.");
                    CompleteTask();
                    ExecuteNextTask();
                }
            }
            else
            {
                CompleteTask();
                ExecuteNextTask();
            }
        }

        private static void WaitForCompilation()
        {
            if (!EditorApplication.isCompiling)
            {
                Debug.Log("Compilation finished.");
                EditorApplication.update -= WaitForCompilation;
                CompleteTask();
                ExecuteNextTask();
            }
        }

        private static void CompleteTask()
        {
            Tasks.Dequeue();
            _isExecuting = false;
            SaveState();
            ClearCompletedTasks();
        }

        private static void OnEditorUpdate()
        {
            if (!_isExecuting && Tasks.Count > 0)
                ExecuteNextTask();
        }

        private static void OnAfterAssemblyReload()
        {
            _isExecuting = false;
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
            string tasksJson = SessionState.GetString("TaskQueueState", "");
            Debug.Log("Loading tasks: " + tasksJson);
            if (!string.IsNullOrEmpty(tasksJson))
            {
                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                try
                {
                    var tasksList = JsonConvert.DeserializeObject<List<Task>>(tasksJson, settings);
                    Tasks.Clear();
                    if (tasksList != null)
                        foreach (var task in tasksList)
                            Tasks.Enqueue(task);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error deserializing tasks: " + ex.Message);
                }
            }
            else
                Debug.Log("No tasks found in SessionState.");
        }

        public static void ClearCompletedTasks()
        {
            if (Tasks.Count == 0)
            {
                SessionState.EraseString("TaskQueueState");
                _isExecuting = false;
                Debug.Log("All tasks completed. TaskQueue and SessionState cleared.");
            }
        }
    }
}
