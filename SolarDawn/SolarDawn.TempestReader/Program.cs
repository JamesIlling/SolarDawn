using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using SolarDawn.ServiceDefaults;
using SolarDawn.Shared;
using SolarDawn.TempestReader.WeatherFlowWebsocketModel;

namespace SolarDawn.TempestReader
{
    internal class Program
    {
        static void Main(string[] args)
        {


            var hostBuilder = CreateHostBuilder(args);

            var token = hostBuilder.Configuration.GetValue<string>("TOKEN");
            var deviceId = hostBuilder.Configuration.GetValue<int>("DEVICE_ID");
            var stationId = hostBuilder.Configuration.GetValue<int>("STATION_ID");
            var serviceProvider = hostBuilder.Services.BuildServiceProvider();

            var client = serviceProvider.GetService<WeatherFlowWebsocketClient>();

            var uri = new Uri($"wss://ws.weatherflow.com/swd/data?token={token}");

            client.Listen(uri, deviceId, stationId);
        }



        private static IHostApplicationBuilder CreateHostBuilder(string[] args)
        {
            var host = new HostApplicationBuilder(args);
            host.AddServiceDefaults();

            host.Services.AddHttpClient<MessageForwarder>(x => x.BaseAddress = new Uri("https+http://apiservice"));
            host.Services.AddSingleton<WeatherFlowWebsocketClient>();
            host.Services.AddLogging(x =>
            {
                x.AddConsole();
                x.AddFilter(level => true);
            });

            return host;
        }
    }
}
