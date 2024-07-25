using SolarDawn.TempestReader.WeatherFlowWebsocketModel.Events;
using System.Text.Json;
using FluentAssertions;

namespace SolarDawn.TempestReader.Tests.WeatherFlowWebsocketModel
{
    public class LightningStrikeEventTests
    {

        [Fact]
        public async Task LightningStrikeEvent_CanBeDeserializedTimestamp()
        {
            var message = await File.ReadAllTextAsync(@"WebSocketMessages/LightningStrikeEvent.json");
            var lightningStrikeEvent = JsonSerializer.Deserialize<LightningStrikeEvent>(message);
            lightningStrikeEvent.Should().NotBeNull();
            lightningStrikeEvent!.OccuredAt.Should().Be(new DateTime(2017, 04, 27, 19, 47, 25));

        }

        [Fact]
        public async Task LightningStrikeEvent_CanBeDeserializedDeviceId()
        {
            var message = await File.ReadAllTextAsync(@"WebSocketMessages/LightningStrikeEvent.json");
            var lightningStrikeEvent = JsonSerializer.Deserialize<LightningStrikeEvent>(message);
            lightningStrikeEvent.Should().NotBeNull();
            lightningStrikeEvent!.DeviceId.Should().Be(1110);

        }

        [Fact]
        public async Task LightningStrikeEvent_CanBeDeserializedType()
        {
            var message = await File.ReadAllTextAsync(@"WebSocketMessages/LightningStrikeEvent.json");
            var lightningStrikeEvent = JsonSerializer.Deserialize<RainStartEvent>(message);
            lightningStrikeEvent.Should().NotBeNull();
            lightningStrikeEvent!.Type.Should().Be(LightningStrikeEvent.MessageType.Trim('\"'));

        }

        [Fact]
        public async Task LightningStrikeEvent_CanBeDeserializedDistance()
        {
            var message = await File.ReadAllTextAsync(@"WebSocketMessages/LightningStrikeEvent.json");
            var lightningStrikeEvent = JsonSerializer.Deserialize<LightningStrikeEvent>(message);
            lightningStrikeEvent.Should().NotBeNull();
            lightningStrikeEvent!.Distance.Should().Be(27);

        }

        [Fact]
        public async Task LightningStrikeEvent_CanBeDeserializedEnergy()
        {
            var message = await File.ReadAllTextAsync(@"WebSocketMessages/LightningStrikeEvent.json");
            var lightningStrikeEvent = JsonSerializer.Deserialize<LightningStrikeEvent>(message);
            lightningStrikeEvent.Should().NotBeNull();
            lightningStrikeEvent!.Energy.Should().Be(3848);

        }

        [Fact]
        public Task LightningStrikeEvent_Type_StoredValue()
        {
            var message = new LightningStrikeEvent { Type = LightningStrikeEvent.MessageType, Event = [1493322445, 27, 3848] };

            message.Type.Should().Be(LightningStrikeEvent.MessageType);
            return Task.CompletedTask;
        }
    }

}
