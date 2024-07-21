using Websocket.Client;

namespace SolarDawn.TempestReader;

public interface IWebsocketClient : IDisposable
{
    void Send(string serialize);

    IObservable<DisconnectionInfo> DisconnectionHappened { get; set; }
    IObservable<ReconnectionInfo> ReconnectionHappened { get; set; }
    TimeSpan ReconnectTimeout { get; set; }
    IObservable<ResponseMessage> MessageReceived { get; set; }
    void Start();
}