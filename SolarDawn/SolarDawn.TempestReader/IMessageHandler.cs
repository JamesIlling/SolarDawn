using Websocket.Client;

namespace SolarDawn.TempestReader;

public interface IMessageHandler
{
    void ConnectionOpenHandler(ResponseMessage message);
    void LightningStrikeEventHandler(ResponseMessage message);
    void RainStartEventHandler(ResponseMessage message);
    void StationOnlineEventHandler(ResponseMessage message);
    void StationOfflineEventHandler(ResponseMessage message);
    void AcknowledgementHandler(ResponseMessage message, int stationId);
    void Observation<T>(ResponseMessage message) where T : class, IObservation;
}