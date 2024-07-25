using System.Text.Json.Serialization;

namespace SolarDawn.TempestReader.WeatherFlowWebsocketModel.Events
{
    /// <summary>
    /// Rain start event response message. [type = evt_precip]
    /// </summary>

    public class RainStartEvent
    {
        public const string MessageType = "\"evt_precip\"";

        /// <summary>
        /// The device id.
        /// </summary>
        [JsonPropertyName("device_id")]
        public int DeviceId { get; set; }

        /// <summary>
        /// The message type.
        /// </summary>
        [JsonPropertyName("type")]
        public required string Type { get; set; }

        /// <summary>
        /// The event data.
        /// </summary>
        /// <remarks>
        /// i.e. "evt": [1597166429] which I believe is the time Epoch in seconds. (32bit)
        /// </remarks>
        [JsonPropertyName("evt")]
        public List<int>? Event { get; set; }

        /// <summary>
        /// The time at which the rain event occured at.
        /// </summary>
        public DateTime? OccuredAt
        {
            get
            {
                if (Event != null)
                {
                    return DateTimeOffset.FromUnixTimeSeconds(Event[0]).DateTime;
                }

                return null;
            }
        }
    }
}
