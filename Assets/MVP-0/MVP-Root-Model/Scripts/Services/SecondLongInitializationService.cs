using System;
using System.Threading.Tasks;

namespace MVP_0.MVP_Root_Model.Scripts.Services
{
    public class SecondLongInitializationService
    {
        private bool _isInitialized;

        public Task Init()
        {
            if (!_isInitialized)
                return InitializeAsync();

            Console.WriteLine("LongInitializationService is already initialized.");
            return Task.CompletedTask;
        }

        private async Task InitializeAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(3)); // Задержка на 3 секунд для имитации
            _isInitialized = true;
        }
    }
}