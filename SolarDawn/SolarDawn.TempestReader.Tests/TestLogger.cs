using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace SolarDawn.TempestReader.Tests;

public class TestLogger<T> : ILogger<T>
{
    private readonly ITestOutputHelper _output;

    public readonly List<Tuple<LogLevel, string>> Messages = [];

    public TestLogger(ITestOutputHelper output)
    {
        _output = output;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        Messages.Add(new Tuple<LogLevel, string>(logLevel, state?.ToString() ?? string.Empty));
        _output.WriteLine($"{logLevel}: {state}");
    }
}