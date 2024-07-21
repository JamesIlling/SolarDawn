using System.Text.Json;
using FluentAssertions;
using SolarDawn.TempestReader.WeatherFlowWebsocketModel.Events;

namespace SolarDawn.TempestReader.Tests.WeatherFlowWebsocketModel
{
    public class RainStartEventTests
    {
        [Fact]
        public async Task RainStartEvent_CanBeDeserializedTimestamp()
        {
            var message = await File.ReadAllTextAsync(@"WebSocketMessages/RainStartEventWithEvent.json");
            var rainStartEvent = JsonSerializer.Deserialize<RainStartEvent>(message);

            rainStartEvent.OccuredAt.Should().Be(new DateTime(2017, 04, 27, 19, 47, 25));

        }

        [Fact]
        public async Task RainStartEvent_CanBeDeserializedDeviceId()
        {
            var message = await File.ReadAllTextAsync(@"WebSocketMessages/RainStartEventWithEvent.json");
            var rainStartEvent = JsonSerializer.Deserialize<RainStartEvent>(message);

            rainStartEvent.DeviceId.Should().Be(1110);

        }

        [Fact]
        public async Task RainStartEvent_CanBeDeserializedType()
        {
            var message = await File.ReadAllTextAsync(@"WebSocketMessages/RainStartEventWithEvent.json");
            var rainStartEvent = JsonSerializer.Deserialize<RainStartEvent>(message);

            rainStartEvent.Type.Should().Be(RainStartEvent.MessageType.Trim('\"'));

        }
    }
}
