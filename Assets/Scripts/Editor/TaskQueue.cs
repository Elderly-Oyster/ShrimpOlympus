using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [InitializeOnLoad]
    public static class TaskQueue
    {
        private static Queue<Task> tasks = new Queue<Task>();
        private static bool isExecuting = false;

        static TaskQueue()
        {
            LoadState();
            EditorApplication.update += OnEditorUpdate;
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
        }

        public static void EnqueueTask(Task task)
        {
            tasks.Enqueue(task);
            SaveState();
            ExecuteNextTask();
        }

        private static void ExecuteNextTask()
        {
            if (isExecuting || tasks.Count == 0)
                return;

            isExecuting = true;
            var task = tasks.Peek();
            task.Execute();

            if (task.WaitForCompilation)
            {
                EditorApplication.update += WaitForCompilation;
            }
            else
            {
                tasks.Dequeue();
                isExecuting = false;
                SaveState();
                ExecuteNextTask();
            }
        }

        private static void WaitForCompilation()
        {
            if (!EditorApplication.isCompiling)
            {
                EditorApplication.update -= WaitForCompilation;
                tasks.Dequeue();
                isExecuting = false;
                SaveState();
                ExecuteNextTask();
            }
        }

        private static void OnEditorUpdate()
        {
            if (!isExecuting && tasks.Count > 0)
            {
                ExecuteNextTask();
            }
        }

        private static void OnAfterAssemblyReload()
        {
            LoadState();
        }

        private static void SaveState()
        {
            List<string> serializedTasks = new List<string>();
            foreach (var task in tasks)
            {
                var taskData = new TaskData
                {
                    TypeName = task.GetType().AssemblyQualifiedName,
                    SerializedData = JsonUtility.ToJson(task)
                };
                serializedTasks.Add(JsonUtility.ToJson(taskData));
            }
            string tasksJson = JsonUtility.ToJson(new TaskQueueData { Tasks = serializedTasks });
            SessionState.SetString("TaskQueueState", tasksJson);
        }

        private static void LoadState()
        {
            string tasksJson = SessionState.GetString("TaskQueueState", "");
            if (!string.IsNullOrEmpty(tasksJson))
            {
                var taskQueueData = JsonUtility.FromJson<TaskQueueData>(tasksJson);
                tasks.Clear();
                foreach (var taskJson in taskQueueData.Tasks)
                {
                    var taskData = JsonUtility.FromJson<TaskData>(taskJson);
                    Type taskType = Type.GetType(taskData.TypeName);
                    if (taskType != null)
                    {
                        var task = JsonUtility.FromJson(taskData.SerializedData, taskType) as Task;
                        if (task != null)
                        {
                            tasks.Enqueue(task);
                        }
                    }
                }
            }
        }

        [Serializable]
        private class TaskQueueData
        {
            public List<string> Tasks;
        }

        [Serializable]
        private class TaskData
        {
            public string TypeName;
            public string SerializedData;
        }
    }
}
