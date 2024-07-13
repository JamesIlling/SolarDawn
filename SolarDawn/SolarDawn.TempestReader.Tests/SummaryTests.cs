using System.Text.Json;
using FluentAssertions;
using SolarDawn.TempestReader.WeatherFlowWebsocketModel;

namespace SolarDawn.TempestReader.Tests;

public class SummaryTests
{

    [Fact]
    public async Task Summary_CanBeDeserializedFromMessage()
    {
        var message = await File.ReadAllTextAsync(@"WebSocketMessages/Summary.json");
        var summary = JsonSerializer.Deserialize<Summary>(message);
        summary.Should().NotBeNull();

        summary.Should().NotBeNull();
        summary.PressureTrend.Should().Be("falling");
        summary.OneHourLightningStrikeCount.Should().Be(0);
        summary.ThreeHourLightningStrikeCount.Should().Be(0);
        summary.OneHourPrecipitationTotal.Should().Be(0.0);
        summary.LocalPrecipitationAccumulationForYesterday.Should().Be(0);
        summary.PrecipitationAnalysisTypeForYesterday.Should().Be(0);
        summary.FeelsLike.Should().Be(19.4);
        summary.HeatIndex.Should().Be(19.4);
        summary.WindChill.Should().Be(19.4);
        summary.WetBulbTemperature.Should().Be(13.1);
        summary.WetBulbGlobeTemperature.Should().Be(15.4);
        summary.AirDensity.Should().Be(1.19625);
        summary.DeltaT.Should().Be(6.3);
        summary.PrecipitationMinutesToday.Should().Be(0);
        summary.PrecipitationMinutesYesterday.Should().Be(0);
    }
}