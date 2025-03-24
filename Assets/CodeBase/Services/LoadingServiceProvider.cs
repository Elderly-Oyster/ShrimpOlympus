using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using R3;

namespace CodeBase.Services
{
    public class LoadingServiceProvider
    {
        public readonly Dictionary<string, Func<UniTask>> Commands = new();

        private Subject<Dictionary<string, Func<UniTask>>> _commandsDictionarySubject;
        private readonly TaskCompletionSource<bool> _registrationCompleted = new();

        public Task WaitForRegistration => _registrationCompleted.Task;

        public void RegisterCommands(string name, Func<UniTask> command)
        {
            Commands.TryAdd(command.Method.Name, command);
            _commandsDictionarySubject.OnNext(new Dictionary<string, Func<UniTask>> { { name, command } });
        }

        public void UnregisterCommands(string name)
        {
            Commands.Remove(name);
        }

        public void ResetRegistrationProgress()
        {
            _registrationCompleted.SetResult(false);
        }
        
        public void CompleteRegistration()
        {
            _registrationCompleted.TrySetResult(true);
        }
    }
}