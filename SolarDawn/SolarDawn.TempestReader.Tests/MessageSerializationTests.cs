using System.Text.Json;
using FluentAssertions;
using SolarDawn.TempestReader.WeatherFlowWebsocketModel;

namespace SolarDawn.TempestReader.Tests
{
    public class MessageSerializationTests
    {
        [Fact]
        public async Task Acknowledgement_CanBeDeserializedFromMessage()
        {
            var message = await File.ReadAllTextAsync(@"WebSocketMessages/Acknowledgement.json");
            var ack = JsonSerializer.Deserialize<Acknowledgement>(message);
            ack.Should().NotBeNull();
            ack.Id.Should().Be("145787");
            ack.Type.Should().Be(Acknowledgement.MessageType.Trim('\"'));
        }

    }
}