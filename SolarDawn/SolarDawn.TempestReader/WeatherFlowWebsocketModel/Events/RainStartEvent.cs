using System.Text.Json.Serialization;

namespace SolarDawn.TempestReader.WeatherFlowWebsocketModel.Events
{
    /// <summary>
    /// Rain start event response message. [type = evt_precip]
    /// </summary>
    /// <remarks>
    /// Unfortunately the WeatherFlow Smart Weather API WebSocket reference (https://weatherflow.github.io/SmartWeather/api/ws.html)
    /// was out of date when putting this class together and as such the class was built using the message returned 
    /// and https://json2csharp.com/.
    /// </remarks>
    public class RainStartEvent
    {
        public const string MessageType = "\"evt_precip\"";

        /// <summary>
        /// The device id.
        /// </summary>
        [JsonPropertyName("device_id")]
        public int DeviceId { get; set; }

        /// <summary>
        /// The source.
        /// </summary>
        /// <remarks>
        /// i.e. "source":"mqtt" - not exactly sure what this is.
        /// </remarks>
        [JsonPropertyName("source")]
        public required string Source { get; set; }

        /// <summary>
        /// The device serial number.
        /// </summary>
        [JsonPropertyName("serial_number")]
        public string? SerialNumber { get; set; }

        /// <summary>
        /// The message type.
        /// </summary>
        [JsonPropertyName("type")]
        public required string Type { get; set; }

        /// <summary>
        /// The hub serial number.
        /// </summary>
        [JsonPropertyName("hub_sn")]
        public string? HubSerialNumber { get; set; }

        /// <summary>
        /// The event data.
        /// </summary>
        /// <remarks>
        /// i.e. "evt": [1597166429] which I believe is the time Epoch in seconds. (32bit)
        /// </remarks>
        [JsonPropertyName("evt")]
        public required List<int> Event { get; set; }

        /// <summary>
        /// The time at which the rain event occured at.
        /// </summary>
        public DateTime OccuredAt => DateTimeOffset.FromUnixTimeSeconds(Event[0]).DateTime;
    }
}
