using Microsoft.Extensions.Logging;
using SolarDawn.Shared;
using SolarDawn.TempestReader.WeatherFlowWebsocketModel;
using System.Net.Http.Json;

namespace SolarDawn.TempestReader
{
    public class MessageForwarder
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
            _logger.LogInformation("Processing {observation}",observation);

            var uri = new Uri(_client.BaseAddress, "/api/Observation");
            _logger.LogInformation("Url:{uri}", uri.AbsolutePath);
            
            if (_client == null)
            {
                _logger.LogError("HttpClient not configured");
                return;
            }


            var obs = new WeatherObservation
            {
                Timestamp = observation.OccuredAt,
                Temperature = observation.AirTemperature,
                RainAccumulated = observation.RainAccumulation,
                RainAccumulationDay = observation.LocalDayRainAccumulation
            };

            
            _logger.LogInformation("Reprocessed Observation {obs}", observation);
            _client.PostAsync(uri, JsonContent.Create(obs)).GetAwaiter().GetResult();


        }
    }
}
