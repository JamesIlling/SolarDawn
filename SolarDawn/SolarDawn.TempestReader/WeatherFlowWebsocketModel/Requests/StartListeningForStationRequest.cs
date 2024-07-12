namespace SolarDawn.TempestReader.WeatherFlowWebsocketModel.Requests;

public record StartListeningForStationRequest(string type, int station_id, string id)
{
    public override string ToString()
    {
        return $"{{ type = {type}, station_id = {station_id}, id = {id} }}";
    }
}