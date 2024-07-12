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

        // Identifier Keys
        statusMessage!.Type.Should().Be(StatusMessage.MessageType.Trim('\"'));
        statusMessage.Source.Should().Be(StatusMessage.SourceText.Trim('\"'));

        statusMessage.Status.Should().NotBeNull();
        statusMessage.Status!.Code.Should().Be(0);
        statusMessage.Status.Message.Should().Be("SUCCESS");
        statusMessage.DeviceId.Should().Be(354105);

        statusMessage.Summary.Should().NotBeNull();
        statusMessage.Summary!.PressureTrend.Should().Be("falling");
        statusMessage.Summary!.OneHourLightningStrikeCount.Should().Be(0);
        statusMessage.Summary!.ThreeHourLightningStrikeCount.Should().Be(0);
        statusMessage.Summary!.OneHourPrecipitationTotal.Should().Be(0.0);
        statusMessage.Summary!.LocalPrecipitationAccumulationForYesterday.Should().Be(0);
        statusMessage.Summary!.PrecipitationAnalysisTypeForYesterday.Should().Be(0);
        statusMessage.Summary!.FeelsLike.Should().Be(19.4);
        statusMessage.Summary!.HeatIndex.Should().Be(19.4);
        statusMessage.Summary!.WindChill.Should().Be(19.4);
        statusMessage.Summary!.WetBulbTemperature.Should().Be(13.1);
        statusMessage.Summary!.WetBulbGlobeTemperature.Should().Be(15.5);
        statusMessage.Summary!.AirDensity.Should().Be(1.19625);
        statusMessage.Summary!.DeltaT.Should().Be(6.3);
        statusMessage.Summary!.PrecipitationMinutesToday.Should().Be(0);
        statusMessage.Summary!.PrecipitationMinutesYesterday.Should().Be(0);
        statusMessage.Summary!.LastLightningStrikeDistance.Should().BeNull();
        statusMessage.Summary!.LastLightningStrikeEpoch.Should().BeNull();
        statusMessage.Summary!.FinalLocalPrecipitationAccumulationForYesterday.Should().BeNull();

        statusMessage.FirstObservation.OccuredAt.Should().Be(DateTime.UnixEpoch.AddSeconds(1720800882));
        statusMessage.FirstObservation.AirTemperature.Should().Be(19.4);
        statusMessage.FirstObservation.AverageStrikeDistance.Should().Be(0);
        statusMessage.FirstObservation.Battery.Should().Be(2.72);
        statusMessage.FirstObservation.Illuminance.Should().Be(10323);
        statusMessage.FirstObservation.LocalDayRainAccumulation.Should().Be(0);
        statusMessage.FirstObservation.LocalDayRainAccumulationFinal.Should().Be(0);
        statusMessage.FirstObservation.LocalDayRainAccumulationFinal.Should().Be(0);
        statusMessage.FirstObservation.PrecipitationAnalysisType.Should().Be(PrecipitationAnalysis.None);
        statusMessage.FirstObservation.PrecipitationType.Should().Be(Precipitation.None);
        statusMessage.FirstObservation.Pressure.Should().Be(1004.6);
        statusMessage.FirstObservation.RainAccumulation.Should().Be(0);
        statusMessage.FirstObservation.RainAccumulationFinal.Should().Be(0);
        statusMessage.FirstObservation.RelativeHumidity.Should().Be(48);
        statusMessage.FirstObservation.ReportInterval.Should().Be(1);
        statusMessage.FirstObservation.SolarRadiation.Should().Be(86);
        statusMessage.FirstObservation.StrikeCount.Should().Be(0);
        statusMessage.FirstObservation.UV.Should().Be(0.78);
        statusMessage.FirstObservation.WindLull.Should().Be(0);
        statusMessage.FirstObservation.WindAvg.Should().Be(0.18);
        statusMessage.FirstObservation.WindGust.Should().Be(0.52);
        statusMessage.FirstObservation.WindDirection.Should().Be(31);
        statusMessage.FirstObservation.WindSampleInterval.Should().Be(3);
    }
}