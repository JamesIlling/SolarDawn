using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SolarDawn.TempestReader.WeatherFlowWebsocketModel.Events
{
    public class ConnectionOpenEvent
    {
        public const string MessageType = "\"evt_strike\"";
        /// <summary>
        /// The message type.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
