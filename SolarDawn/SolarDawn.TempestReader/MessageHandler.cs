using System.Numerics;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using SolarDawn.TempestReader.WeatherFlowWebsocketModel;
using SolarDawn.TempestReader.WeatherFlowWebsocketModel.Events;
using Websocket.Client;

namespace SolarDawn.TempestReader;

public class MessageHandler : IMessageHandler
{
    private const string TimeFormat = "u";
    private const string ReceivedMessage = "Received {msg}";
    private readonly IProcessObservation _forwarder;
    private readonly ILogger<MessageHandler> _logger;

    public MessageHandler(ILogger<MessageHandler> logger, IProcessObservation forwarder)
    {
        _logger = logger;
        _forwarder = forwarder;
    }

    public void ConnectionOpenHandler(ResponseMessage message)
    {
        _logger.LogDebug(ReceivedMessage, message);
        _logger.LogInformation("Connection Opened");
    }

    public void LightningStrikeEventHandler(ResponseMessage message)
    {
        _logger.LogDebug(ReceivedMessage, message);
        var lightningStrikeEvent =
            JsonSerializer.Deserialize<LightningStrikeEvent>(message.Text ?? string.Empty);
        if (lightningStrikeEvent != null)
        {
            _logger.LogInformation("Lightning strike event occured at {time}, {distance}km away",
                lightningStrikeEvent.OccuredAt.ToString(TimeFormat), lightningStrikeEvent.Distance);
        }
    }

    public void RainStartEventHandler(ResponseMessage message)
    {
        _logger.LogDebug(ReceivedMessage, message);
        var rainStartEvent = JsonSerializer.Deserialize<RainStartEvent>(message.Text ?? string.Empty);
        if (rainStartEvent is { OccuredAt: not null })
        {
            _logger.LogInformation("Rain event occured {time}",
                rainStartEvent.OccuredAt.Value.ToString(TimeFormat));
        }
    }


    public void StationOnlineEventHandler(ResponseMessage message)
    {
        _logger.LogDebug(ReceivedMessage, message);
        _logger.LogInformation("Station Online");
    }

    public void StationOfflineEventHandler(ResponseMessage message)
    {
        _logger.LogDebug(ReceivedMessage, message);
        _logger.LogInformation("Station Offline");
    }

    public void AcknowledgementHandler(ResponseMessage message, int stationId)
    {
        _logger.LogDebug(ReceivedMessage, message);
        var ack = JsonSerializer.Deserialize<Acknowledgement>(message.Text ?? string.Empty);
        if (ack != null)
        {
            var stationDevice = ack.Id.Equals(stationId.ToString()) ? "station" : "device";
            _logger.LogInformation("Start/Stop Listening for {station_or_device_id}: {acknowledgement_id}",
                stationDevice, ack.Id);
        }
    }

    public void Observation<T>(ResponseMessage message) where T : class, IObservation
    {
        _logger.LogDebug(ReceivedMessage, message);
        var processedMessage = JsonSerializer.Deserialize<T>(message.Text ?? string.Empty);
        if (processedMessage != null)
        {
            _forwarder.ProcessObservation(processedMessage.FirstObservation);
        }
    }
}