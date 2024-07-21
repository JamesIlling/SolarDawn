using FluentAssertions;
using SolarDawn.TempestReader.WeatherFlowWebsocketModel.Events;
using SolarDawn.TempestReader.WeatherFlowWebsocketModel.Requests;

namespace SolarDawn.TempestReader.Tests.WeatherFlowWebsocketModel
{
    public class EventTests
    {
        [Fact]
        public void ConnectionOpenEvents_CanStoreType()
        {
            var coe = new ConnectionOpenEvent()
            {
                Type = ConnectionOpenEvent.MessageType
            };

            coe.Type.Should().Be(ConnectionOpenEvent.MessageType);
        }
    }

    public class RequestTests
    {

        [Fact]
        public void StartListeningForDeviceRequest_ToString_ReturnsJson()
        {
            var slde = new StartListeningForDeviceRequest("StartListeningEvent", 1, "2");
            slde.ToString().Should().Be("{ type = StartListeningEvent, device_id = 1, id = 2 }");
        }

        [Fact]
        public void StartListeningForStationRequest_ToString_ReturnsJson()
        {
            var slse = new StartListeningForStationRequest("StartListeningEvent", 1, "2");
            slse.ToString().Should().Be("{ type = StartListeningEvent, station_id = 1, id = 2 }");

        }
    }
}
