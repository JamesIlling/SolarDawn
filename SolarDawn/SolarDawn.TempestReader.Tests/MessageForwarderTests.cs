using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using SolarDawn.Shared;
using SolarDawn.TempestReader.Tests.Helpers;
using SolarDawn.TempestReader.WeatherFlowWebsocketModel;
using Xunit.Abstractions;

namespace SolarDawn.TempestReader.Tests
{
    public class MessageForwarderTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public MessageForwarderTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerOptions.Default)
        {
            PropertyNameCaseInsensitive = true
        };

        private static bool ContainsExpectedContent(HttpContent? content, Observation observation)
        {
            var jsonText = content?.ReadAsStringAsync().GetAwaiter().GetResult();
            if (jsonText == null)
            {
                return false;
            }

            var json = JsonSerializer.Deserialize<WeatherObservation>(jsonText, JsonSerializerOptions);
            if (json == null)
            {
                return false;
            }
            return Math.Abs(json.RainAccumulated - observation.RainAccumulation) < 0.01
                && Math.Abs(json.Temperature - observation.AirTemperature) < 0.01
                && json.Timestamp == observation.OccuredAt
                && Math.Abs(json.RainAccumulationDay - observation.LocalDayRainAccumulation) < 0.01;

        }


        private (TestHttpMessageHandler handler, Observation observation, MessageForwarder forwarder, TestLogger<MessageForwarder> logger) Build(bool withBaseAddress = true)
        {
            var messageHandler = new TestHttpMessageHandler();
            messageHandler.Responses.Add(new HttpResponseMessage(HttpStatusCode.OK));
            HttpClient httpClient;
            if (withBaseAddress)
            {
                httpClient = new HttpClient(messageHandler)
                {
                    BaseAddress = new Uri("https://localhost:5000")
                };
            }
            else
            {
                httpClient = new HttpClient(messageHandler);
            }

            var logger = new TestLogger<MessageForwarder>(_outputHelper);
            var forwarder = new MessageForwarder(logger, httpClient);
            var observationMessage = File.ReadAllText("WebSocketMessages\\Observation.json");
            var json = JsonSerializer.Deserialize<List<double?>>(observationMessage);
            if (json == null)
            {
                Assert.Fail("Unable to convert test json file into object.");
            }
            var observation = new Observation(json);
            return (messageHandler, observation, forwarder, logger);
        }


        [Fact]
        public void MessageForwarder_SendsHttpMessage()
        {
            var (messageHandler, observation, forwarder, _) = Build();

            forwarder.ProcessObservation(observation);

            messageHandler.Requests.Any(request =>

                request.Method == HttpMethod.Post
                    && request.RequestUri?.AbsoluteUri == "https://localhost:5000/api/Observation"
                    && ContainsExpectedContent(request.Content, observation)
            ).Should().BeTrue();
        }

        [Fact]
        public void MessageForwarder_NoBaseAddress_DoesNotSendsHttpMessage()
        {
            var (messageHandler, observation, forwarder, _) = Build(false);

            forwarder.ProcessObservation(observation);

            messageHandler.Requests.Count.Should().Be(0);
        }

        [Fact]
        public void MessageForwarder_LogsUserMessage()
        {
            var (_, observation, forwarder, logger) = Build();

            forwarder.ProcessObservation(observation);

            logger.Messages.Any(x => x.Item1 == LogLevel.Information
                                     && x.Item2.Contains("Forwarding Observation for 2024-07-12 16:14:42Z to SolarDawnApi",
                                         StringComparison.OrdinalIgnoreCase)).Should().BeTrue();
        }

        [Fact]
        public void MessageForwarder_NoBaseAddress_DoesNotLogUserMessage()
        {
            var (_, observation, forwarder, logger) = Build(false);

            forwarder.ProcessObservation(observation);

            logger.Messages.Any(x => x.Item1 == LogLevel.Information
                                     && x.Item2.Contains("Forwarding Observation for 2024-07-12 16:14:42Z to SolarDawnApi",
                                         StringComparison.OrdinalIgnoreCase)).Should().BeFalse();
        }
    }
}

