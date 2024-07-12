using System.Text.Json.Serialization;

namespace SolarDawn.TempestReader.WeatherFlowWebsocketModel.Events
{
    /// <summary>
    /// Lightning strike event response message. [type = evt_strike]
    /// </summary>
    public class LightningStrikeEvent
    {
        public const string MessageType = "\"evt_strike\"";
        /// <summary>
        /// The message type.
        /// </summary>
        [JsonPropertyName("type")]
        public required string Type { get; set; }

        /// <summary>
        /// The event data. 
        /// </summary>
        /// <remarks>
        /// i.e. "evt": [1597165492, 42, -1714, 1] - I believe these are as follows.
        /// Time - Epoch in seconds. (32bit)
        /// Distance
        /// Energy
        /// Unknown
        /// </remarks>
        [JsonPropertyName("evt")]
        public required List<int> Event { get; set; }

        /// <summary>
        /// The device id.
        /// </summary>
        [JsonPropertyName("device_id")]
        public int DeviceId { get; set; }

        /// <summary>
        /// The time at which the lightning strike event occured at,
        /// </summary>
        public DateTime OccuredAt => DateTimeOffset.FromUnixTimeSeconds(Event[0]).DateTime;

        /// <summary>
        /// The distance of the lightning strike (km).
        /// </summary>
        public int Distance => Event[1];

        /// <summary>
        /// The energy of the lightning strike.
        /// </summary>
        public int Energy => Event[2];
    }
}