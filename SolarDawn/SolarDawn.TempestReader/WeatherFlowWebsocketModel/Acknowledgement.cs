using System.Text.Json.Serialization;

namespace SolarDawn.TempestReader.WeatherFlowWebsocketModel
{
    /// <summary>
    /// Acknowledgement response message. [type = ack]
    /// </summary>
    public class Acknowledgement
    {
        public const string MessageType = "\"ack\"";

        /// <summary>
        /// The message type.
        /// </summary>
        [JsonPropertyName("type")]
        public required string Type { get; set; }

        /// <summary>
        /// The id.
        /// </summary>
        [JsonPropertyName("id")]
        public required string Id { get; set; }
    }
}