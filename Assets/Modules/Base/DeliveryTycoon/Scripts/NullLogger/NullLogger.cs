using System;
using Microsoft.Extensions.Logging;

namespace Modules.Base.DeliveryTycoon.Scripts.NullLogger
{
    internal class NullLogger : ILogger
    {
        public static readonly NullLogger Instance = new();

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => false;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter) { }
    }

    internal class NullLoggerFactory : ILoggerFactory
    {
        public void AddProvider(ILoggerProvider provider) { }

        public ILogger CreateLogger(string categoryName) => NullLogger.Instance;

        public void Dispose() { }
    }
}