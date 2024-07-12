namespace SolarDawn.TempestReader.WeatherFlowWebsocketModel;

/// <summary>
/// The precipitation analysis type.
/// </summary>
public enum PrecipitationAnalysis
{
    None = 0,
    RainCheckWithUserDisplayOn = 1,
    RainCheckWithUserDisplayOff = 2,
    Unknown = -1
}