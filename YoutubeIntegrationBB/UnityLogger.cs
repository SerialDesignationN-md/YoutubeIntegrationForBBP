// UnityLogger.cs

using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Debug = UnityEngine.Debug;

public class UnityLogger : Microsoft.Extensions.Logging.ILogger
{
    private readonly string _categoryName;
    private readonly BepInEx.Logging.ManualLogSource _bepInExLogger =
        BepInEx.Logging.Logger.CreateLogSource("UnityLogger");

    public UnityLogger(string categoryName) => _categoryName = categoryName;

    public IDisposable BeginScope<TState>(TState state) => default!;

    public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel) => true;

    public void Log<TState>(
        Microsoft.Extensions.Logging.LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    )
    {
        if (!IsEnabled(logLevel))
            return;

        string message = formatter(state, exception);

        if (exception != null)
            message += $" | Exception: {exception}";

        switch (logLevel)
        {
            case Microsoft.Extensions.Logging.LogLevel.Critical:
                Debug.LogError(message);
                _bepInExLogger.LogError(message);
                break;
            case Microsoft.Extensions.Logging.LogLevel.Error:
                Debug.LogError(message);
                _bepInExLogger.LogError(message);
                break;
            case Microsoft.Extensions.Logging.LogLevel.Warning:
                Debug.LogWarning(message);
                _bepInExLogger.LogWarning(message);
                break;
            case Microsoft.Extensions.Logging.LogLevel.Information:
                Debug.Log(message);
                _bepInExLogger.LogInfo(message);
                break;
            case Microsoft.Extensions.Logging.LogLevel.Debug:
            case Microsoft.Extensions.Logging.LogLevel.Trace:
                Debug.Log(message);
                _bepInExLogger.LogDebug(message);
                break;
            default:
                Debug.Log(message);
                _bepInExLogger.LogInfo(message);
                break;
        }
    }
}

public class UnityLoggerFactory : ILoggerFactory
{
    private readonly ConcurrentDictionary<string, ILogger> _loggers = new();

    public ILogger CreateLogger(string categoryName) =>
        _loggers.GetOrAdd(categoryName, name => new UnityLogger(name));

    public void AddProvider(ILoggerProvider provider)
    {
        // Not needed for this adapter
    }

    public void Dispose() => _loggers.Clear();
}

public static class BepInExLoggerFactoryExtensions
{
    public static ILogger<T> CreateLogger<T>(this ILoggerFactory factory) =>
        new LoggerWrapper<T>(factory.CreateLogger(typeof(T).FullName!));

    private class LoggerWrapper<T> : ILogger<T>
    {
        private readonly ILogger _inner;

        public LoggerWrapper(ILogger inner) => _inner = inner;

        public IDisposable BeginScope<TState>(TState state) => _inner.BeginScope(state);

        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel) =>
            _inner.IsEnabled(logLevel);

        public void Log<TState>(
            Microsoft.Extensions.Logging.LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception?, string> formatter
        ) => _inner.Log(logLevel, eventId, state, exception, formatter);
    }
}