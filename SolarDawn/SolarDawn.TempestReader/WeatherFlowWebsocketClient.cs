using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SolarDawn.TempestReader.WeatherFlowWebsocketModel;
using SolarDawn.TempestReader.WeatherFlowWebsocketModel.Events;
using SolarDawn.TempestReader.WeatherFlowWebsocketModel.Requests;

namespace SolarDawn.TempestReader;

// Base was taken from https://github.com/tgolla/DotNetWeatherFlowTempestAPIWebsocketExample/tree/master
public class WeatherFlowWebsocketClient : IDisposable
{
    private readonly IMessageHandler _handler;

    private readonly ILogger<WeatherFlowWebsocketClient> _logger;
    private int _deviceId;
    private int _stationId;
    private bool? _started;
    private readonly IWebsocketClient _websocketClient;

    public WeatherFlowWebsocketClient(ILogger<WeatherFlowWebsocketClient> logger, IMessageHandler handler, IWebsocketClient client)
    {
        _logger = logger;
        _handler = handler;
        _websocketClient = client;
    }

    public void Configure(int deviceId, int stationId)
    {
        _deviceId = deviceId;
        _stationId = stationId;

        _logger.LogDebug("{deviceId}-{stationId}", deviceId, stationId);
        _websocketClient.ReconnectTimeout = TimeSpan.FromMinutes(2);
        _websocketClient.ReconnectionHappened.Subscribe(info =>
        {
            try
            {
                _logger.LogDebug("Reconnection happened, type: {info}", info.Type);

                var startListeningForDevice =
                    new StartListeningForDeviceRequest("listen_start", deviceId, deviceId.ToString());

                _websocketClient.Send(JsonSerializer.Serialize(startListeningForDevice));

                var startListeningForStation =
                    new StartListeningForStationRequest("listen_start_events", stationId, stationId.ToString());

                _websocketClient.Send(JsonSerializer.Serialize(startListeningForStation));
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unable to connect to WeatherFlow.");
            }
        });


        _websocketClient.DisconnectionHappened.Subscribe(info =>
        {
            var message = info.Exception?.Message ?? info.CloseStatusDescription ?? info.Type.ToString();
            _logger.LogError("Disconnection happened, type: {message}", message);
        });


        var gate = new object();

        _websocketClient.MessageReceived
            .Where(msg => msg.Text?.Contains(ConnectionOpenEvent.MessageType) ?? false)
            .ObserveOn(TaskPoolScheduler.Default)
            .Synchronize(gate)
            .Subscribe(_handler.ConnectionOpenHandler);

        _websocketClient.MessageReceived
            .Where(msg => msg.Text?.Contains(Acknowledgement.MessageType) ?? false)
            .ObserveOn(TaskPoolScheduler.Default)
            .Synchronize(gate)
            .Subscribe(msg => _handler.AcknowledgementHandler(msg, stationId));

        _websocketClient.MessageReceived
            .Where(msg =>
                (msg.Text?.Contains(StatusMessage.MessageType) ?? false) &&
                (msg.Text?.Contains(StatusMessage.SourceText) ?? false))
            .ObserveOn(TaskPoolScheduler.Default)
            .Synchronize(gate)
            .Subscribe(_handler.Observation<StatusMessage>);

        _websocketClient.MessageReceived
            .Where(msg =>
                (msg.Text?.Contains(SummaryMessage.MessageType) ?? false) &&
                (msg.Text?.Contains(SummaryMessage.SourceText) ?? false))
            .ObserveOn(TaskPoolScheduler.Default)
            .Synchronize(gate)
            .Subscribe(_handler.Observation<SummaryMessage>);

        _websocketClient.MessageReceived
            .Where(msg => msg.Text?.Contains(LightningStrikeEvent.MessageType) ?? false)
            .ObserveOn(TaskPoolScheduler.Default)
            .Synchronize(gate)
            .Subscribe(_handler.LightningStrikeEventHandler);

        _websocketClient.MessageReceived
            .Where(msg => msg.Text?.Contains(RainStartEvent.MessageType) ?? false)
            .ObserveOn(TaskPoolScheduler.Default)
            .Synchronize(gate)
            .Subscribe(_handler.RainStartEventHandler);

        _websocketClient.MessageReceived
            .Where(msg => msg.Text?.Contains("\"evt_station_online\"") ?? false)
            .ObserveOn(TaskPoolScheduler.Default)
            .Synchronize(gate)
            .Subscribe(_handler.StationOnlineEventHandler);

        _websocketClient.MessageReceived
            .Where(msg => msg.Text?.Contains("\"evt_station_offline\"") ?? false)
            .ObserveOn(TaskPoolScheduler.Default)
            .Synchronize(gate)
            .Subscribe(_handler.StationOfflineEventHandler);
    }


    public void Start()
    {
        if (_websocketClient == null)
        {
            throw new InvalidOperationException(
                "The WeatherFlowWebsocketClient has not been configured, please run configure first");
        }

        _started = true;
        _websocketClient.Start();
    }

    public void Stop()
    {

        if (_websocketClient == null)
        {
            throw new InvalidOperationException(
                "The WeatherFlowWebsocketClient has not been configured, please run configure first");
        }

        var stopListeningForDevice =
            new StartListeningForDeviceRequest("listen_stop", _deviceId, _deviceId.ToString());

        _websocketClient.Send(JsonSerializer.Serialize(stopListeningForDevice));

        var stopListeningForStation =
            new StartListeningForStationRequest("listen_stop_events", _stationId, _stationId.ToString());

        _websocketClient.Send(JsonSerializer.Serialize(stopListeningForStation));
        _started = false;
    }

    protected virtual void Dispose(bool disposing)
    {

        if (_websocketClient != null &&
            (_started ?? false))
        {
            Stop();
        }

        _websocketClient?.Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
