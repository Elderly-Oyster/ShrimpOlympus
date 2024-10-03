using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [InitializeOnLoad]
    public static class TaskQueue
    {
        private static readonly Queue<Task> Tasks = new Queue<Task>();
        private static bool isExecuting = false;

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
                if (isExecuting || Tasks.Count == 0) return;

                isExecuting = true;
                var task = Tasks.Peek();
                task.Execute();

                if (task.WaitForCompilation)
                    EditorApplication.update += WaitForCompilation;
                else
                {
                    Tasks.Dequeue();
                    isExecuting = false;
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
                EditorApplication.update -= WaitForCompilation;
                Tasks.Dequeue();
                isExecuting = false;
                SaveState();
                ExecuteNextTask();
            }
        }

        private static void OnEditorUpdate()
        {
            if (!isExecuting && Tasks.Count > 0) 
                ExecuteNextTask();
        }

        private static void OnAfterAssemblyReload() => LoadState();

        private static void SaveState()
        {
            List<string> serializedTasks = new List<string>();
            foreach (var task in Tasks)
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
                Tasks.Clear();
                foreach (var taskJson in taskQueueData.Tasks)
                {
                    var taskData = JsonUtility.FromJson<TaskData>(taskJson);
                    Type taskType = Type.GetType(taskData.TypeName);
                    if (taskType != null)
                    {
                        var task = JsonUtility.FromJson(taskData.SerializedData, taskType) as Task;
                        if (task != null)
                        {
                            Tasks.Enqueue(task);
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
