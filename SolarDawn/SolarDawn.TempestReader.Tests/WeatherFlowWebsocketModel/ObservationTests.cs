using System.Text.Json;
using FluentAssertions;
using SolarDawn.TempestReader.WeatherFlowWebsocketModel;

namespace SolarDawn.TempestReader.Tests.WeatherFlowWebsocketModel;

public class ObservationTests
{
    [Fact]
    public async Task Observation_Construction()
    {
        var message = await File.ReadAllTextAsync(@"WebSocketMessages/Observation.json");
        var data = JsonSerializer.Deserialize<List<double?>>(message);
        data.Should().NotBeNull();
        var observation = new Observation(data!);
        observation.Should().NotBeNull();

        observation.OccuredAt.Should().Be(DateTime.UnixEpoch.AddSeconds(1720800882));
        observation.AirTemperature.Should().Be(19.4);
        observation.AverageStrikeDistance.Should().Be(0);
        observation.Battery.Should().Be(2.72);
        observation.Illuminance.Should().Be(10323);
        observation.LocalDayRainAccumulation.Should().Be(0);
        observation.LocalDayRainAccumulationFinal.Should().Be(0);
        observation.LocalDayRainAccumulationFinal.Should().Be(0);
        observation.PrecipitationAnalysisType.Should().Be(PrecipitationAnalysis.None);
        observation.PrecipitationType.Should().Be(Precipitation.None);
        observation.Pressure.Should().Be(1004.6);
        observation.RainAccumulation.Should().Be(0);
        observation.RainAccumulationFinal.Should().Be(0);
        observation.RelativeHumidity.Should().Be(48);
        observation.ReportInterval.Should().Be(1);
        observation.SolarRadiation.Should().Be(86);
        observation.StrikeCount.Should().Be(0);
        observation.UV.Should().Be(0.78);
        observation.WindLull.Should().Be(0);
        observation.WindAvg.Should().Be(0.18);
        observation.WindGust.Should().Be(0.52);
        observation.WindDirection.Should().Be(31);
        observation.WindSampleInterval.Should().Be(3);
    }

    [Fact]
    public async Task Observation_ToStringReturnExpectedText()
    {
        var message = await File.ReadAllTextAsync(@"WebSocketMessages/Observation.json");
        var data = JsonSerializer.Deserialize<List<double?>>(message);
        data.Should().NotBeNull();
        var observation = new Observation(data!);
        observation.ToString().Trim().Should().Be("""
                                                  AirTemperature: 19.4
                                                   AverageStrikeDistance: 0
                                                   Battery: 2.72
                                                   Illuminance: 10323
                                                   LocalDayRainAccumulation: 0
                                                   LocalDayRainAccumulationFinal: 0
                                                   OccuredAt: 12/07/2024 16:14:42
                                                   PrecipitationAnalysisType: None
                                                   PrecipitationType: None
                                                   Pressure: 1004.6
                                                   RainAccumulation: 0
                                                   RainAccumulationFinal: 0
                                                   RelativeHumidity: 48
                                                   ReportInterval: 1
                                                   SolarRadiation: 86
                                                   StrikeCount: 0
                                                   UV: 0.78
                                                   WindAvg: 0.18
                                                   WindDirection: 31
                                                   WindGust: 0.52
                                                   WindLull: 0
                                                   WindSampleInterval: 3
                                                  """);
    }
}