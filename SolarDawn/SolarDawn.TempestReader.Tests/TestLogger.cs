using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace SolarDawn.TempestReader.Tests;

public class TestLogger<T> : ILogger<T>, IDisposable
{
    private ITestOutputHelper _output;

    public TestLogger(ITestOutputHelper output)
    {
        _output = output;

    }

    public List<Tuple<LogLevel, string>> Messages = [];

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return this;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        Messages.Add(new Tuple<LogLevel, string>(logLevel, state.ToString()));
        _output.WriteLine($"{logLevel}: {state}");
    }

    public void Dispose()
    {

    }
}