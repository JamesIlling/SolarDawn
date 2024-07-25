using System.Reactive.Subjects;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SolarDawn.TempestReader.Tests.Helpers;
using SolarDawn.TempestReader.WeatherFlowWebsocketModel;
using SolarDawn.TempestReader.WeatherFlowWebsocketModel.Events;
using SolarDawn.TempestReader.WeatherFlowWebsocketModel.Requests;
using Websocket.Client;
using Xunit.Abstractions;

namespace SolarDawn.TempestReader.Tests;

public class WeatherFlowWebsocketClientTests
{
    private readonly ITestOutputHelper _outputHelper;

    public WeatherFlowWebsocketClientTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    private const int Delay = 100;

    private static Mock<IWebsocketClient> SetupWebSocketClient()
    {
        var mockClient = new Mock<IWebsocketClient>();
        mockClient.SetupProperty(x => x.DisconnectionHappened, new Subject<DisconnectionInfo>());
        mockClient.SetupProperty(x => x.ReconnectionHappened, new Subject<ReconnectionInfo>());
        mockClient.SetupProperty(x => x.MessageReceived, new Subject<ResponseMessage>());
        return mockClient;
    }

    private (Mock<IWebsocketClient> client, TestLogger<WeatherFlowWebsocketClient> logger, WeatherFlowWebsocketClient
        weatherFlow, Mock<IMessageHandler> messageHandler) Setup()
    {
        var mockClient = SetupWebSocketClient();
        var logger = new TestLogger<WeatherFlowWebsocketClient>(_outputHelper);
        var messageHandler = new Mock<IMessageHandler>();

        var weatherFlowClient = new WeatherFlowWebsocketClient(logger, messageHandler.Object, mockClient.Object);
        return (client: mockClient, logger, weatherFlow: weatherFlowClient, messageHandler);
    }

    private WeatherFlowWebsocketClient SetupNoClient()
    {
        var logger = new TestLogger<WeatherFlowWebsocketClient>(_outputHelper);
        var messageHandler = new Mock<IMessageHandler>();

        var weatherFlowClient = new WeatherFlowWebsocketClient(logger, messageHandler.Object, null!);
        return weatherFlowClient;
    }

    private (Mock<IWebsocketClient> client, TestLogger<WeatherFlowWebsocketClient> logger, WeatherFlowWebsocketClient
       weatherFlow) SetupSendThrowsException()
    {
        var mockClient = SetupWebSocketClient();
        mockClient.Setup(x => x.Send(It.IsAny<string>())).Throws<InvalidOperationException>();
        var logger = new TestLogger<WeatherFlowWebsocketClient>(_outputHelper);
        var messageHandler = new Mock<IMessageHandler>();

        var weatherFlowClient = new WeatherFlowWebsocketClient(logger, messageHandler.Object, mockClient.Object);
        return (client: mockClient, logger, weatherFlow: weatherFlowClient);
    }

    [Fact]
    public void Configure_SetsReconnectionTimeout()
    {
        var (webSocketClient, _, weatherFlowClient, _) = Setup();

        weatherFlowClient.Configure(1, 2);

        webSocketClient.VerifySet(x => x.ReconnectTimeout = TimeSpan.FromMinutes(2));
    }

    [Fact]
    public void Configure_LogsDeviceAndStationIds()
    {
        const int deviceId = 1;
        const int stationId = 2;
        var (_, logger, weatherFlowClient, _) = Setup();

        weatherFlowClient.Configure(deviceId, stationId);

        logger.Messages.Any(x => x.Item1 == LogLevel.Debug && x.Item2.Contains($"{deviceId}-{stationId}")).Should()
            .BeTrue();
    }

    [Fact]
    public void Configure_SetsReconnectionSubscription()
    {
        var (webSocketClient, _, weatherFlowClient, _) = Setup();

        weatherFlowClient.Configure(1, 2);

        webSocketClient.Verify(x => x.ReconnectionHappened, Times.Once);
    }

    [Fact]
    public void Configure_SetsDisconnectionSubscription()
    {
        var (webSocketClient, _, weatherFlowClient, _) = Setup();

        weatherFlowClient.Configure(1, 2);

        webSocketClient.Verify(x => x.DisconnectionHappened, Times.Once);
    }

    [Fact]
    public void Configure_SetsMessageReceivedHandler()
    {
        var (webSocketClient, _, weatherFlowClient, _) = Setup();

        weatherFlowClient.Configure(1, 2);

        webSocketClient.Verify(x => x.MessageReceived, Times.Exactly(8));
    }

    [Fact]
    public void Start_SendsStartToClient()
    {
        var (webSocketClient, _, weatherFlowClient, _) = Setup();

        weatherFlowClient.Configure(1, 2);
        weatherFlowClient.Start();

        webSocketClient.Verify(x => x.Start(), Times.Once);
    }

    [Fact]
    public void Start_HandlesNoClient()
    {
        var weatherFlowClient = SetupNoClient();
        Assert.Throws<InvalidOperationException>(() => weatherFlowClient.Start());
    }

    [Fact]
    public void Stop_HandlesNoClient()
    {
        var weatherFlowClient = SetupNoClient();
        Assert.Throws<InvalidOperationException>(() => weatherFlowClient.Stop());
    }

    [Fact]
    public void Stop_SendsStopListeningMessages()
    {
        var (webSocketClient, _, weatherFlowClient, _) = Setup();

        weatherFlowClient.Configure(1, 2);
        weatherFlowClient.Start();
        weatherFlowClient.Stop();

        var device = new StartListeningForDeviceRequest("listen_stop", 1, "1");
        var station = new StartListeningForStationRequest("listen_stop_events", 2, "2");
        var deviceMessage = JsonSerializer.Serialize(device);
        var stationMessage = JsonSerializer.Serialize(station);
        webSocketClient.Verify(x => x.Send(deviceMessage), Times.Exactly(1));
        webSocketClient.Verify(x => x.Send(stationMessage), Times.Exactly(1));
    }


    [Fact]
    public async Task Message_ConnectionOpenIsProcessed()
    {
        var (webSocketClient, _, weatherFlowClient, messageHandler) = Setup();

        weatherFlowClient.Configure(1, 2);
        weatherFlowClient.Start();
        var connection = new ConnectionOpenEvent { Type = ConnectionOpenEvent.MessageType.Trim('\"') };
        SendMessage(webSocketClient, connection);
        await Task.Delay(Delay);

        messageHandler.Verify(x => x.ConnectionOpenHandler(It.IsAny<ResponseMessage>()), Times.Once);
    }

    [Fact]
    public async Task Message_AcknowledgementIsProcessed()
    {
        var (webSocketClient, _, weatherFlowClient, messageHandler) = Setup();

        weatherFlowClient.Configure(1, 2);
        weatherFlowClient.Start();

        SendMessage(webSocketClient, "WebSocketMessages/Acknowledgement.json");

        await Task.Delay(Delay);

        messageHandler.Verify(x => x.AcknowledgementHandler(It.IsAny<ResponseMessage>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task Message_StationOnlineIsProcessed()
    {
        var (webSocketClient, _, weatherFlowClient, messageHandler) = Setup();

        weatherFlowClient.Configure(1, 2);
        weatherFlowClient.Start();

        SendRawMessage(webSocketClient, "{\"evt_station_online\"}");

        await Task.Delay(Delay);

        messageHandler.Verify(x => x.StationOnlineEventHandler(It.IsAny<ResponseMessage>()), Times.Once);
    }

    [Fact]
    public async Task Message_StationOfflineIsProcessed()
    {
        var (webSocketClient, _, weatherFlowClient, messageHandler) = Setup();

        weatherFlowClient.Configure(1, 2);
        weatherFlowClient.Start();

        SendRawMessage(webSocketClient, "{\"evt_station_offline\"}");

        await Task.Delay(Delay);

        messageHandler.Verify(x => x.StationOfflineEventHandler(It.IsAny<ResponseMessage>()), Times.Once);
    }

    [Fact]
    public async Task Message_LightningStrikeEventIsProcessed()
    {
        var (webSocketClient, _, weatherFlowClient, messageHandler) = Setup();

        weatherFlowClient.Configure(1, 2);
        weatherFlowClient.Start();
        SendMessage(webSocketClient, "WebSocketMessages/LightningStrikeEvent.json");

        await Task.Delay(Delay);

        messageHandler.Verify(x => x.LightningStrikeEventHandler(It.IsAny<ResponseMessage>()), Times.Once);
    }

    [Fact]
    public async Task Message_RainStartEventIsProcessed()
    {
        var (webSocketClient, _, weatherFlowClient, messageHandler) = Setup();

        weatherFlowClient.Configure(1, 2);
        weatherFlowClient.Start();
        SendMessage(webSocketClient, "WebSocketMessages/RainStartEventWithEvent.json");

        await Task.Delay(Delay);

        messageHandler.Verify(x => x.RainStartEventHandler(It.IsAny<ResponseMessage>()), Times.Once);
    }


    [Fact]
    public async Task Message_RainStartEventNoEvtIsProcessed()
    {
        var (webSocketClient, _, weatherFlowClient, messageHandler) = Setup();

        weatherFlowClient.Configure(1, 2);
        weatherFlowClient.Start();
        SendMessage(webSocketClient, "WebSocketMessages/RainStartEvent.json");

        await Task.Delay(Delay);

        messageHandler.Verify(x => x.RainStartEventHandler(It.IsAny<ResponseMessage>()), Times.Once);
    }

    [Fact]
    public async Task Message_StatusMessageProcessed()
    {
        var (webSocketClient, _, weatherFlowClient, messageHandler) = Setup();

        weatherFlowClient.Configure(1, 2);
        weatherFlowClient.Start();

        SendMessage(webSocketClient, "WebSocketMessages/StatusMessage.json");

        await Task.Delay(Delay);

        messageHandler.Verify(x => x.Observation<StatusMessage>(It.IsAny<ResponseMessage>()), Times.Once);
    }

    private static void SendMessage<T>(Mock<IWebsocketClient> webSocketClient, T message)
    {
        SendRawMessage(webSocketClient, JsonSerializer.Serialize(message));
    }

    private static void SendMessage(Mock<IWebsocketClient> webSocketClient, string path)
    {
        var message = File.ReadAllText(path);
        SendRawMessage(webSocketClient, message);
    }

    private static void SendRawMessage(Mock<IWebsocketClient> webSocketClient, string message)
    {

        var responseMessage = ResponseMessage.TextMessage(message);
        var subject = webSocketClient.Object.MessageReceived as Subject<ResponseMessage>;
        if (subject == null)
        {
            Assert.Fail("Unable to send message");
        }
        subject.OnNext(responseMessage);
    }

    [Fact]
    public void Reconnection_SendsStartListeningMessages()
    {
        var (webSocketClient, _, weatherFlowClient, _) = Setup();

        weatherFlowClient.Configure(1, 2);
        var responseMessage = new ReconnectionInfo(ReconnectionType.NoMessageReceived);
        var subject = webSocketClient.Object.ReconnectionHappened as Subject<ReconnectionInfo>;
        if (subject == null)
        {
            Assert.Fail();
        }
        subject.OnNext(responseMessage);

        var device = new StartListeningForDeviceRequest("listen_start", 1, "1");
        var station = new StartListeningForStationRequest("listen_start_events", 2, "2");
        var deviceMessage = JsonSerializer.Serialize(device);
        var stationMessage = JsonSerializer.Serialize(station);
        webSocketClient.Verify(x => x.Send(deviceMessage), Times.Exactly(1));
        webSocketClient.Verify(x => x.Send(stationMessage), Times.Exactly(1));
    }

    [Fact]
    public void Reconnection_HandlesErrors()
    {
        var (webSocketClient, logger, weatherFlowClient) = SetupSendThrowsException();

        weatherFlowClient.Configure(1, 2);
        var responseMessage = new ReconnectionInfo(ReconnectionType.Lost);
        var subject = webSocketClient.Object.ReconnectionHappened as Subject<ReconnectionInfo>;
        if (subject == null)
        {
            Assert.Fail();
        }
        subject.OnNext(responseMessage);

        var device = new StartListeningForDeviceRequest("listen_start", 1, "1");
        var station = new StartListeningForStationRequest("listen_start_events", 2, "2");
        var deviceMessage = JsonSerializer.Serialize(device);
        var stationMessage = JsonSerializer.Serialize(station);
        webSocketClient.Verify(x => x.Send(deviceMessage), Times.Exactly(1));
        webSocketClient.Verify(x => x.Send(stationMessage), Times.Exactly(0));
        logger.Messages.Any(x => x.Item1 == LogLevel.Critical && x.Item2.Contains("Unable to connect to WeatherFlow.")).Should().BeTrue();
    }

    [Fact]
    public void Disconnection_LogsErrors()
    {
        var (webSocketClient, logger, weatherFlowClient, _) = Setup();

        weatherFlowClient.Configure(1, 2);
        var responseMessage = DisconnectionInfo.Create(DisconnectionType.ByUser, null, null);
        var subject = webSocketClient.Object.DisconnectionHappened as Subject<DisconnectionInfo>;
        subject!.OnNext(responseMessage);

        logger.Messages.Any(x => x.Item1 == LogLevel.Error && x.Item2.Contains("Disconnection happened, type:")).Should().BeTrue();
    }

    [Fact]
    public void Dispose_Handles_NoClient()
    {
        var weatherFlowWebsocketClient = SetupNoClient();
        weatherFlowWebsocketClient.Dispose();
        Assert.True(true); // Assertation that we get to the end as exception would cause error

    }

    [Fact]
    public void Dispose_RunningClient_CallsStop()
    {
        var (webSocketClient, _, weatherFlow, _) = Setup();
        weatherFlow.Configure(1, 2);
        weatherFlow.Start();
        weatherFlow.Dispose();

        var device = new StartListeningForDeviceRequest("listen_stop", 1, "1");
        var station = new StartListeningForStationRequest("listen_stop_events", 2, "2");
        var deviceMessage = JsonSerializer.Serialize(device);
        var stationMessage = JsonSerializer.Serialize(station);
        webSocketClient.Verify(x => x.Send(deviceMessage), Times.Exactly(1));
        webSocketClient.Verify(x => x.Send(stationMessage), Times.Exactly(1));
    }
}