using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SolarDawn.TempestReader.Tests.Helpers;
using SolarDawn.TempestReader.WeatherFlowWebsocketModel;
using Websocket.Client;
using Xunit.Abstractions;

namespace SolarDawn.TempestReader.Tests;

public class MessageHandlerTests
{
    private readonly ITestOutputHelper _output;

    public MessageHandlerTests(ITestOutputHelper output)
    {
        _output = output;
    }

    private (TestLogger<MessageHandler> logger, MessageHandler handler, ResponseMessage message) Build(
        string message = "TestMessage")
    {
        var logger = new TestLogger<MessageHandler>(_output);
        var messageForwarder = new Mock<IProcessObservation>();
        var messageHandler = new MessageHandler(logger, messageForwarder.Object);
        return (logger, messageHandler, ResponseMessage.TextMessage(message));
    }


    [Fact]
    public void ConnectionOpenHandler_LogsIncomingMessage()
    {
        var (logger, handler, message) = Build();

        handler.ConnectionOpenHandler(message);

        logger.Messages.Any(x =>
            x.Item1 == LogLevel.Debug &&
            x.Item2.Contains(message.Text!, StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }

    [Fact]
    public void ConnectionOpenHandler_LogsUserMessage()
    {
        var (logger, handler, message) = Build();

        handler.ConnectionOpenHandler(message);

        logger.Messages.Any(x =>
            x.Item1 == LogLevel.Information &&
            x.Item2.Contains("Connection Opened", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }

    [Fact]
    public void StationOnlineEventHandler_LogsIncomingMessage()
    {
        var (logger, handler, message) = Build();

        handler.StationOnlineEventHandler(message);

        logger.Messages.Any(x =>
            x.Item1 == LogLevel.Debug &&
            x.Item2.Contains(message.Text!, StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }

    [Fact]
    public void StationOnlineEventHandler_LogsUserMessage()
    {
        var (logger, handler, message) = Build();

        handler.StationOnlineEventHandler(message);

        logger.Messages.Any(x =>
            x.Item1 == LogLevel.Information &&
            x.Item2.Contains("Station Online", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }

    [Fact]
    public void StationOfflineEventHandler_LogsIncomingMessage()
    {
        var (logger, handler, message) = Build();

        handler.StationOfflineEventHandler(message);

        logger.Messages.Any(x =>
            x.Item1 == LogLevel.Debug &&
            x.Item2.Contains(message.Text!, StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }

    [Fact]
    public void StationOfflineEventHandler_LogsUserMessage()
    {
        var (logger, handler, message) = Build();

        handler.StationOfflineEventHandler(message);

        logger.Messages.Any(x =>
            x.Item1 == LogLevel.Information &&
            x.Item2.Contains("Station Offline", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }

    [Fact]
    public void AcknowledgementHandler_LogsIncomingMessage()
    {
        var messageText = File.ReadAllText("WebsocketMessages\\Acknowledgement.json");
        var (logger, handler, message) = Build(messageText);

        handler.AcknowledgementHandler(message, 0);

        logger.Messages.Any(x =>
            x.Item1 == LogLevel.Debug &&
            x.Item2.Contains(message.Text!, StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }

    [Fact]
    public void AcknowledgementHandler_LogsIncomingStationMessage()
    {
        var messageText = File.ReadAllText("WebsocketMessages\\Acknowledgement.json");
        var (logger, handler, message) = Build(messageText);

        handler.AcknowledgementHandler(message, 145787);

        logger.Messages.Any(x =>
            x.Item1 == LogLevel.Information &&
            x.Item2.Contains("Start/Stop Listening for station: 145787", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }

    [Fact]
    public void AcknowledgementHandler_LogsIncomingDeviceMessage()
    {
        var messageText = File.ReadAllText("WebsocketMessages\\Acknowledgement.json");
        var (logger, handler, message) = Build(messageText);

        handler.AcknowledgementHandler(message, 0);

        logger.Messages.Any(x =>
            x.Item1 == LogLevel.Information &&
            x.Item2.Contains("Start/Stop Listening for device: 145787", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }
    [Fact]
    public void AcknowledgementHandler_HandlesNullMessage()
    {
        var (logger, handler, message) = Build("null");

        handler.AcknowledgementHandler(message, 0);

        logger.Messages.Any(x =>
            x.Item1 == LogLevel.Information).Should().BeFalse();
    }

    [Fact]
    public void RainStartEventHandler_HandlesNullMessage()
    {
        var (logger, handler, message) = Build("null");

        handler.RainStartEventHandler(message);

        logger.Messages.Any(x =>
            x.Item1 == LogLevel.Information).Should().BeFalse();
    }

    [Fact]
    public void RainStartEventHandler_LogsIncomingMessage()
    {
        var messageText = File.ReadAllText("WebSocketMessages\\RainStartEvent.json");
        var (logger, handler, message) = Build(messageText);

        handler.RainStartEventHandler(message);

        logger.Messages.Any(x =>
            x.Item1 == LogLevel.Debug &&
            x.Item2.Contains(message.Text!, StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }

    [Fact]
    public void RainStartEventHandler_WithoutEvent_DoesNotLogUserMessage()
    {
        var messageText = File.ReadAllText("WebSocketMessages\\RainStartEvent.json");
        var (logger, handler, message) = Build(messageText);

        handler.RainStartEventHandler(message);

        logger.Messages.Any(x =>
            x.Item1 == LogLevel.Information).Should().BeFalse();
    }

    [Fact]
    public void RainStartEventHandler_WithEvent_LogsUserMessage()
    {
        var messageText = File.ReadAllText("WebSocketMessages\\RainStartEventWithEvent.json");
        var (logger, handler, message) = Build(messageText);

        handler.RainStartEventHandler(message);

        logger.Messages.Any(x =>
            x.Item1 == LogLevel.Information
            && x.Item2.Contains("Rain event occured 2017-04-27 19:47:25Z", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }

    [Fact]
    public void LightningStrikeEventHandler_HandlesNullMessage()
    {
        var (logger, handler, message) = Build("null");

        handler.LightningStrikeEventHandler(message);

        logger.Messages.Any(x =>
            x.Item1 == LogLevel.Information).Should().BeFalse();
    }

    [Fact]
    public void LightningStrikeEventHandler_LogsIncomingMessage()
    {
        var messageText = File.ReadAllText("WebSocketMessages\\LightningStrikeEvent.json");
        var (logger, handler, message) = Build(messageText);

        handler.LightningStrikeEventHandler(message);

        logger.Messages.Any(x =>
            x.Item1 == LogLevel.Debug &&
            x.Item2.Contains(message.Text!, StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }

    [Fact]
    public void LightningStrikeEventHandler_LogsUserMessage()
    {
        var messageText = File.ReadAllText("WebSocketMessages\\LightningStrikeEvent.json");
        var (logger, handler, message) = Build(messageText);

        handler.LightningStrikeEventHandler(message);

        logger.Messages.Any(x =>
            x.Item1 == LogLevel.Information &&
            x.Item2.Contains("Lightning strike event occured at 2017-04-27 19:47:25Z, 27km away", StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }

    [Fact]
    public void Observation_HandlesNullMessage()
    {
        var logger = new TestLogger<MessageHandler>(_output);
        var messageForwarder = new Mock<IProcessObservation>();
        var messageHandler = new MessageHandler(logger, messageForwarder.Object);
        var message = ResponseMessage.TextMessage("null");

        messageHandler.Observation<SummaryMessage>(message);

        messageForwarder.Verify(x => x.ProcessObservation(It.IsAny<Observation>()), Times.Never);
    }

    [Fact]
    public void Observation_SummaryMessage_ForwardsValidMessage()
    {
        var logger = new TestLogger<MessageHandler>(_output);
        var messageForwarder = new Mock<IProcessObservation>();
        var messageHandler = new MessageHandler(logger, messageForwarder.Object);
        var message = ResponseMessage.TextMessage(File.ReadAllText("WebSocketMessages\\SummaryMessage.json"));

        messageHandler.Observation<SummaryMessage>(message);

        messageForwarder.Verify(x => x.ProcessObservation(It.IsAny<Observation>()), Times.Once);
    }

    [Fact]
    public void Observation_StatusMessage_ForwardsValidMessage()
    {
        var logger = new TestLogger<MessageHandler>(_output);
        var messageForwarder = new Mock<IProcessObservation>();
        var messageHandler = new MessageHandler(logger, messageForwarder.Object);
        var message = ResponseMessage.TextMessage(File.ReadAllText("WebSocketMessages\\StatusMessage.json"));

        messageHandler.Observation<StatusMessage>(message);

        messageForwarder.Verify(x => x.ProcessObservation(It.IsAny<Observation>()), Times.Once);
    }

    [Fact]
    public void Observation_SummaryMessage_LogsIncomingMessage()
    {
        var logger = new TestLogger<MessageHandler>(_output);
        var messageForwarder = new Mock<IProcessObservation>();
        var messageHandler = new MessageHandler(logger, messageForwarder.Object);
        var message = ResponseMessage.TextMessage(File.ReadAllText("WebSocketMessages\\SummaryMessage.json"));

        messageHandler.Observation<SummaryMessage>(message);

        logger.Messages.Any(x =>
            x.Item1 == LogLevel.Debug &&
            x.Item2.Contains(message.Text!, StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }

    [Fact]
    public void Observation_StatusMessage_LogsIncomingMessage()
    {
        var logger = new TestLogger<MessageHandler>(_output);
        var messageForwarder = new Mock<IProcessObservation>();
        var messageHandler = new MessageHandler(logger, messageForwarder.Object);
        var message = ResponseMessage.TextMessage(File.ReadAllText("WebSocketMessages\\StatusMessage.json"));

        messageHandler.Observation<StatusMessage>(message);

        logger.Messages.Any(x =>
            x.Item1 == LogLevel.Debug &&
            x.Item2.Contains(message.Text!, StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
    }
}