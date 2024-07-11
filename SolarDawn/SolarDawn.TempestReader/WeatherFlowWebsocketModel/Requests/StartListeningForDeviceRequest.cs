namespace SolarDawn.TempestReader.WeatherFlowWebsocketModel.Requests;

public record StartListeningForDeviceRequest(string type, int device_id, string id)
{
    public override string ToString()
    {
        return $"{{ type = {type}, device_id = {device_id}, id = {id} }}";
    }
}