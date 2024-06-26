using System;
using System.Threading.Tasks;

namespace MVP.MVP_Root_Model.Scripts.Services
{
    public class ThirdLongInitializationService
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
            await Task.Delay(TimeSpan.FromSeconds(1)); // Задержка на 1 секунду для имитации
            _isInitialized = true;
        }
    }
}