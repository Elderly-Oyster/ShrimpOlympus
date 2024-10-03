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
            Tasks.Enqueue(task);
            SaveState();
            ExecuteNextTask();
        }

        private static void ExecuteNextTask()
        {
            while (true)
            {
                if (_isExecuting || Tasks.Count == 0)
                    return;

                _isExecuting = true;
                var task = Tasks.Peek();
                task.Execute();

                if (task.WaitForCompilation)
                {
                    if (EditorApplication.isCompiling)
                    {
                        EditorApplication.update += WaitForCompilation;
                    }
                    else
                    {
                        Tasks.Dequeue();
                        _isExecuting = false;
                        SaveState();
                        ClearCompletedTasks();
                        continue;
                    }
                }
                else
                {
                    Tasks.Dequeue();
                    _isExecuting = false;
                    SaveState();
                    ClearCompletedTasks();
                    continue;
                }

                break;
            }
        }

        private static void WaitForCompilation()
        {
            if (!EditorApplication.isCompiling)
            {
                EditorApplication.update -= WaitForCompilation;
                Tasks.Dequeue();
                _isExecuting = false;
                SaveState();
                ClearCompletedTasks();
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
            _isExecuting = false;
            LoadState();
        }

        private static void SaveState()
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            string tasksJson = JsonConvert.SerializeObject(Tasks.ToList(), settings);
            SessionState.SetString("TaskQueueState", tasksJson);
        }

        private static void LoadState()
        {
            string tasksJson = SessionState.GetString("TaskQueueState", "");
            if (!string.IsNullOrEmpty(tasksJson))
            {
                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                try
                {
                    var tasksList = JsonConvert.DeserializeObject<List<Task>>(tasksJson, settings);
                    Tasks.Clear();
                    foreach (var task in tasksList) 
                        Tasks.Enqueue(task);
                }
                catch (Exception)
                {
                }
            }
        }

        public static void ClearCompletedTasks()
        {
            if (Tasks.Count == 0)
            {
                SessionState.EraseString("TaskQueueState");
                _isExecuting = false;
            }
        }
    }
}
