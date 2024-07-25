using SolarDawn.TempestReader.WeatherFlowWebsocketModel;

namespace SolarDawn.TempestReader;

public interface IProcessObservation
{
    void ProcessObservation(Observation observation);
}