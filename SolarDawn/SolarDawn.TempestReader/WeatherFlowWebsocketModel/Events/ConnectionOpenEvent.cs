using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace SolarDawn.TempestReader.WeatherFlowWebsocketModel.Events
{
    [UsedImplicitly]
    public class ConnectionOpenEvent
    {
        public const string MessageType = "\"connection_opened\"";
        /// <summary>
        /// The message type.
        /// </summary>
        [JsonPropertyName("type")]
        public required string Type { get; set; }
    }
}
