using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SolarDawn.TempestReader.WeatherFlowWebsocketModel;
using SolarDawn.TempestReader.WeatherFlowWebsocketModel.Events;
using SolarDawn.TempestReader.WeatherFlowWebsocketModel.Requests;
using Websocket.Client;

namespace SolarDawn.TempestReader;

// Base was taken from https://github.com/tgolla/DotNetWeatherFlowTempestAPIWebsocketExample/tree/master
public class WeatherFlowWebsocketClient
{

    private readonly ILogger<WeatherFlowWebsocketClient> _logger;
    private readonly MessageHandler _handler;


    public WeatherFlowWebsocketClient(ILogger<WeatherFlowWebsocketClient> logger, MessageHandler handler)
    {
        _logger = logger;
        _handler = handler;
    }






    public void Listen(Uri remoteClient, int deviceId, int stationId)
    {
        _logger.LogDebug("{url}-{deviceId}-{stationId}", remoteClient, deviceId, stationId);
        using (var client = new WebsocketClient(remoteClient))
        {
            Console.WriteLine("Press ESC to stop WebSocket connection...");


            client.ReconnectTimeout = TimeSpan.FromMinutes(2);
            client.ReconnectionHappened.Subscribe(info =>
            {
                try
                {
                    _logger.LogDebug("Reconnection happened, type: {info}", info.Type);

                    var startListeningForDevice =
                        new StartListeningForDeviceRequest("listen_start", deviceId, deviceId.ToString());

                    client.Send(JsonSerializer.Serialize(startListeningForDevice));

                    var startListeningForStation =
                        new StartListeningForStationRequest("listen_start_events", stationId, stationId.ToString());

                    client.Send(JsonSerializer.Serialize(startListeningForStation));
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Unable to connect to WeatherFlow.");
                }
            });


            client.DisconnectionHappened.Subscribe(info =>
            {
                var message = info.Exception?.Message ?? info.CloseStatusDescription ?? info.Type.ToString();
                _logger.LogError("Disconnection happened, type: {message}", message);
            });


            var gate = new object();

            client.MessageReceived
                .Where(msg => msg.Text?.Contains(ConnectionOpenEvent.MessageType) ?? false)
                .ObserveOn(TaskPoolScheduler.Default)
                .Synchronize(gate)
                .Subscribe(_handler.ConnectionOpenHandler);

            client.MessageReceived
                .Where(msg => msg.Text?.Contains(Acknowledgement.MessageType) ?? false)
                .ObserveOn(TaskPoolScheduler.Default)
                .Synchronize(gate)
                .Subscribe(msg => _handler.AcknowledgementHandler(msg, stationId));

            client.MessageReceived
                .Where(msg =>
                    (msg.Text?.Contains(StatusMessage.MessageType) ?? false) &&
                    (msg.Text?.Contains(StatusMessage.SourceText) ?? false))
                .ObserveOn(TaskPoolScheduler.Default)
                .Synchronize(gate)
                .Subscribe(_handler.Observation<StatusMessage>);

            client.MessageReceived
                .Where(msg =>
                    (msg.Text?.Contains(SummaryMessage.MessageType) ?? false) &&
                    (msg.Text?.Contains(SummaryMessage.SourceText) ?? false))
                .ObserveOn(TaskPoolScheduler.Default)
                .Synchronize(gate)
                .Subscribe(_handler.Observation<SummaryMessage>);

            client.MessageReceived
                .Where(msg => msg.Text?.Contains(LightningStrikeEvent.MessageType) ?? false)
                .ObserveOn(TaskPoolScheduler.Default)
                .Synchronize(gate)
                .Subscribe(_handler.LightningStrikeEventHandler);

            client.MessageReceived
                .Where(msg => msg.Text?.Contains(RainStartEvent.MessageType) ?? false)
                .ObserveOn(TaskPoolScheduler.Default)
                .Synchronize(gate)
                .Subscribe(_handler.RainStartEventHandler);

            client.MessageReceived
                .Where(msg => msg.Text?.Contains("\"evt_station_online\"") ?? false)
                .ObserveOn(TaskPoolScheduler.Default)
                .Synchronize(gate)
                .Subscribe(_handler.StationOnlineEventHandler);

            client.MessageReceived
                .Where(msg => msg.Text?.Contains("\"evt_station_offline\"") ?? false)
                .ObserveOn(TaskPoolScheduler.Default)
                .Synchronize(gate)
                .Subscribe(_handler.StationOfflineEventHandler);


            client.Start();

            WaitForKeyStroke([ConsoleKey.Escape]);

            var stopListeningForDevice =
                new StartListeningForDeviceRequest("listen_stop", deviceId, deviceId.ToString());

            client.Send(JsonSerializer.Serialize(stopListeningForDevice));

            var stopListeningForStation =
                new StartListeningForStationRequest("listen_stop_events", stationId, stationId.ToString());

            client.Send(JsonSerializer.Serialize(stopListeningForStation));
        }

        // This final confirmation is here so you can see the DisconnectionHappened message before the window is closed.
        Console.WriteLine("Press RETURN to close application...");
        Console.ReadLine();
    }

    private static void WaitForKeyStroke(IReadOnlyCollection<ConsoleKey> keys)
    {
        var keyMatch = true;

        do
        {
            while (!Console.KeyAvailable)
            {
                // Just waiting around for the user to press a key.
                // Everything else is running on Async threads.
            }

            var keyPressed = Console.ReadKey(true).Key;

            if (keys.Contains(keyPressed))
            {
                keyMatch = false;
            }
        }
        while (keyMatch);
    }
}