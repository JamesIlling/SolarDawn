using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace SolarDawn.TempestReader.WeatherFlowWebsocketModel;

[UsedImplicitly]
public class Summary
{
    /// <summary>
    /// Pressure trend.
    /// </summary>
    /// <remarks>
    /// i.e. "pressure_trend":"steady"
    /// </remarks>
    [JsonPropertyName("pressure_trend")]
    public string? PressureTrend { get; set; }

    /// <summary>
    /// One hour lightning strike count.
    /// </summary>
    [JsonPropertyName("strike_count_1h")]
    public int? OneHourLightningStrikeCount { get; set; }

    /// <summary>
    /// Three hour lightning strike count.
    /// </summary>
    [JsonPropertyName("strike_count_3h")]
    public int? ThreeHourLightningStrikeCount { get; set; }

    /// <summary>
    /// One hour precipitation total.
    /// </summary>
    [JsonPropertyName("precip_total_1h")]
    public double? OneHourPrecipitationTotal { get; set; }

    /// <summary>
    /// Local precipitation accumulation for yesterday.
    /// </summary>
    [JsonPropertyName("precip_accum_local_yesterday")]
    public double? LocalPrecipitationAccumulationForYesterday { get; set; }

    /// <summary>
    /// Precipitation analysis type for yesterday.
    /// </summary>
    [JsonPropertyName("precip_analysis_type_yesterday")]
    public int? PrecipitationAnalysisTypeForYesterday { get; set; }

    /// <summary>
    /// It feels like.
    /// </summary>
    [JsonPropertyName("feels_like")]
    public double? FeelsLike { get; set; }

    /// <summary>
    /// The heat index.
    /// </summary>
    [JsonPropertyName("heat_index")]
    public double? HeatIndex { get; set; }

    /// <summary>
    /// The Dew Point
    /// </summary>
    [JsonPropertyName("wet_bulb_temperature")]
    public double? WetBulbTemperature { get; set; }

    /// <summary>
    /// The Dew Point
    /// </summary>
    [JsonPropertyName("wet_bulb_globe_temperature")]
    public double? WetBulbGlobeTemperature { get; set; }

    /// <summary>
    /// The air density
    /// </summary>
    [JsonPropertyName("air_density")]
    public double? AirDensity { get; set; }

    /// <summary>
    /// The air density
    /// </summary>
    [JsonPropertyName("delta_t")]
    public double? DeltaT { get; set; }

    /// <summary>
    /// Precipitation analysis type for yesterday.
    /// </summary>
    [JsonPropertyName("precip_minutes_local_day")]
    public int? PrecipitationMinutesToday { get; set; }

    /// <summary>
    /// Precipitation analysis type for yesterday.
    /// </summary>
    [JsonPropertyName("precip_minutes_local_yesterday")]
    public int? PrecipitationMinutesYesterday { get; set; }

    /// <summary>
    /// The wind chill.
    /// </summary>
    [JsonPropertyName("wind_chill")]
    public double? WindChill { get; set; }
}