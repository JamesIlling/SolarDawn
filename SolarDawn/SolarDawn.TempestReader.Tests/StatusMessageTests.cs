using System.Text.Json;
using FluentAssertions;
using SolarDawn.TempestReader.WeatherFlowWebsocketModel;

namespace SolarDawn.TempestReader.Tests;

public class StatusMessageTests
{

    [Fact]
    public async Task StatusMessage_CanBeDeserializedFromMessage()
    {
        var message = await File.ReadAllTextAsync(@"WebSocketMessages/StatusMessage.json");
        var statusMessage = JsonSerializer.Deserialize<StatusMessage>(message);
        statusMessage.Should().NotBeNull();

        statusMessage!.Type.Should().Be(StatusMessage.MessageType.Trim('\"'));
        statusMessage.Source.Should().Be(StatusMessage.SourceText.Trim('\"'));
        statusMessage.Status.Should().NotBeNull();
        statusMessage.Summary.Should().NotBeNull();
        statusMessage.FirstObservation.Should().NotBeNull();
        statusMessage.DeviceId.Should().Be(354105);
    }
}