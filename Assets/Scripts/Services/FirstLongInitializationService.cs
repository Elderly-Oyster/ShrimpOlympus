using System;
using System.Threading.Tasks;

namespace Services
{
    public class FirstLongInitializationService
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
            await Task.Delay(TimeSpan.FromSeconds(2)); // Задержка на 2 секунды для имитации
            _isInitialized = true;
        }
    }
}