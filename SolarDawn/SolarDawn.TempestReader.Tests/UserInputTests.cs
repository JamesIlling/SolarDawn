using Xunit.Abstractions;

namespace SolarDawn.TempestReader.Tests;

public class UserInputTests
{
    private ITestOutputHelper _testOutput;

    public UserInputTests(ITestOutputHelper testOutputHelper)
    {
        _testOutput = testOutputHelper;
    }

    [Fact]
    public async Task WaitForKeyStroke_WaitsForKeyPressIfThereIsNone()

    {
        try
        {
            var task = Task.Run(() => UserInput.WaitForKeyStroke([ConsoleKey.Escape]));

            await task.WaitAsync(TimeSpan.FromMilliseconds(100), CancellationToken.None);
            if (task.IsCompleted)
            {
                Assert.Fail();
            }
        }
        catch (TimeoutException) { return; }

        Assert.Fail("Timeout not reached");
    }


    [Theory]
    [InlineData(ConsoleKey.Escape)]
    [InlineData(ConsoleKey.Enter)]
    [InlineData(ConsoleKey.A)]
    public async Task WaitForKeyStroke_ReturnsWhenKeyEntered(ConsoleKey key)
    {
        var keys = new List<ConsoleKey> { key };
        try
        {
            var text = $"{(char)(int)key}";
            Console.SetIn(new StringReader(text));

            var task = Task.Run(() => UserInput.WaitForKeyStroke(keys));
            await task.WaitAsync(TimeSpan.FromMilliseconds(200), CancellationToken.None);
            if (task.IsCompleted)
            {
                return;
            }
            Assert.Fail("Did not complete");
        }
        catch (TimeoutException)
        {
            Assert.Fail("Timeout reached");
        }
    }
}