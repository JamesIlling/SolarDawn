using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace SolarDawn.TempestReader.WeatherFlowWebsocketModel;

[UsedImplicitly]
public class Status
{
    [JsonPropertyName("status_code")]
    public int? Code { get; set; }

    [JsonPropertyName("status_message")]
    public string? Message { get; set; }

}