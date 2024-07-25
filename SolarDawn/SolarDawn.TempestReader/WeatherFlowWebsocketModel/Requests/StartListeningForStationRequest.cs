namespace SolarDawn.TempestReader.WeatherFlowWebsocketModel.Requests;

public record StartListeningForStationRequest(string Type, int StationId, string Id)
{
    public override string ToString()
    {
        return $"{{ type = {Type}, station_id = {StationId}, id = {Id} }}";
    }
}