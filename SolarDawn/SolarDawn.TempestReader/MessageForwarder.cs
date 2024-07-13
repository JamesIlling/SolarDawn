using Microsoft.Extensions.Logging;
using SolarDawn.Shared;
using SolarDawn.TempestReader.WeatherFlowWebsocketModel;
using System.Net.Http.Json;

namespace SolarDawn.TempestReader
{
    public class MessageForwarder : IProcessObservation
    {
        private readonly ILogger<MessageForwarder> _logger;
        private readonly HttpClient _client;

        public MessageForwarder(ILogger<MessageForwarder> logger, HttpClient client)
        {
            _logger = logger;
            _client = client;
        }

        public void ProcessObservation(Observation observation)
        {
            if (_client.BaseAddress != null)
            {
                var uri = new Uri(_client.BaseAddress, "/api/Observation");

                var obs = new WeatherObservation
                {
                    Timestamp = observation.OccuredAt,
                    Temperature = observation.AirTemperature,
                    RainAccumulated = observation.RainAccumulation,
                    RainAccumulationDay = observation.LocalDayRainAccumulation
                };

                _logger.LogInformation("Forwarding Observation for {time} to SolarDawnApi", observation.OccuredAt.ToString("u"));
                _client.PostAsync(uri, JsonContent.Create(obs)).GetAwaiter().GetResult();
            }
        }
    }

    public interface IProcessObservation
    {
        void ProcessObservation(Observation observation);
    }
}
