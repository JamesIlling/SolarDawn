using Websocket.Client;

namespace SolarDawn.TempestReader;

public class WebsocketClientWrapper : IWebsocketClient
{
    private readonly WebsocketClient _client;

    public WebsocketClientWrapper(Uri address)
    {
        _client = new WebsocketClient(address);
    }


    public void Send(string serialize)
    {
        _client.Send(serialize);
    }

    public IObservable<DisconnectionInfo> DisconnectionHappened
    {
        get => _client.DisconnectionHappened;
        set => throw new NotImplementedException();
    }

    public IObservable<ReconnectionInfo> ReconnectionHappened
    {
        get => _client.ReconnectionHappened;
        set => throw new NotImplementedException();
    }

    public TimeSpan ReconnectTimeout { get; set; }
    public IObservable<ResponseMessage> MessageReceived
    {
        get => _client.MessageReceived;
        set => throw new NotImplementedException();
    }

    public void Start()
    {
        _client.Start();
    }

    protected virtual void Dispose(bool disposing)
    {
        _client.Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}