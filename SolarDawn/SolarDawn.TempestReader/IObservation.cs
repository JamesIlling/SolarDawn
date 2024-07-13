using SolarDawn.TempestReader.WeatherFlowWebsocketModel;

namespace SolarDawn.TempestReader;

public interface IObservation
{
    Observation FirstObservation { get; }
}