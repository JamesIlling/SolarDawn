using SolarDawn.TempestReader.WeatherFlowWebsocketModel;
using System.Text.Json;
using FluentAssertions;

namespace SolarDawn.TempestReader.Tests
{
    public class StatusTests
    {
        [Fact]
        public async Task Status_CanBeDeserializedFromMessage()
        {

            var message = await File.ReadAllTextAsync(@"WebSocketMessages/Status.json");
            var status = JsonSerializer.Deserialize<Status>(message);
            status!.Code.Should().Be(0);
            status.Message.Should().Be("SUCCESS");
        }
    }
}
