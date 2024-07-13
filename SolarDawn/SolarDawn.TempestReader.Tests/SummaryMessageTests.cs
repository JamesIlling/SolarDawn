using System.Text.Json;
using FluentAssertions;
using SolarDawn.TempestReader.WeatherFlowWebsocketModel;

namespace SolarDawn.TempestReader.Tests;

public class SummaryMessageTests
{

    [Fact]
    public async Task StatusMessage_CanBeDeserializedFromMessage()
    {
        var message = await File.ReadAllTextAsync(@"WebSocketMessages/SummaryMessage.json");
        var statusMessage = JsonSerializer.Deserialize<SummaryMessage>(message);
        statusMessage.Should().NotBeNull();

        statusMessage!.Type.Should().Be(SummaryMessage.MessageType.Trim('\"'));
        statusMessage.Source.Should().Be(SummaryMessage.SourceText.Trim('\"'));
        statusMessage.Summary.Should().NotBeNull();
        statusMessage.FirstObservation.Should().NotBeNull();
        statusMessage.HubSerialNumber.Should().Be("HB-00155510");
        statusMessage.FirmwareRevision.Should().Be(176);
        statusMessage.SerialNumber.Should().Be("ST-00144517");
        statusMessage.DeviceId.Should().Be(354105);
    }
}