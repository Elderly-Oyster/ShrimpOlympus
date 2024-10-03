using System.Collections.Generic;
using System.Linq;
using Editor.Tasks.Abstract;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;

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
                    EditorApplication.update += WaitForCompilation;
                else
                {
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

        private static void OnAfterAssemblyReload() => LoadState();

        private static void SaveState()
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            string tasksJson = JsonConvert.SerializeObject(Tasks.ToList(), settings);
            SessionState.SetString("TaskQueueState", tasksJson);
        }

        private static void LoadState()
        {
            SessionState.EraseString("TaskQueueState"); 
            string tasksJson = SessionState.GetString("TaskQueueState", "");
            if (!string.IsNullOrEmpty(tasksJson))
            {
                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                var tasksList = JsonConvert.DeserializeObject<List<Task>>(tasksJson, settings);
                Tasks.Clear();
                foreach (var task in tasksList) 
                    Tasks.Enqueue(task);
            }
        }
    }
}
