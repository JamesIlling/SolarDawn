using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using SolarDawn.ServiceDefaults;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SolarDawn.TempestReader;

[ExcludeFromCodeCoverage]
public static class Program
{
    private static void Main(string[] args)
    {
        var hostBuilder = CreateHostBuilder(args);


        var deviceId = hostBuilder.Configuration.GetValue<int>("DEVICE_ID");
        var stationId = hostBuilder.Configuration.GetValue<int>("STATION_ID");

        var serviceProvider = hostBuilder.Services.BuildServiceProvider();

        using var client = serviceProvider.GetRequiredService<WeatherFlowWebsocketClient>();


        client.Configure(deviceId, stationId);
        client.Start();

        Console.WriteLine("Press Enter to Exit");
        UserInput.WaitForKeyStroke([ConsoleKey.Escape, ConsoleKey.Enter]);

        client.Stop();
    }

    private static HostApplicationBuilder CreateHostBuilder(string[] args)
    {
        var host = new HostApplicationBuilder(args);
        host.AddServiceDefaults();
        host.Services.AddHttpClient<IProcessObservation, MessageForwarder>(client =>
            client.BaseAddress = new Uri("https+http://apiservice"));
        host.Services.RemoveAll<IHttpMessageHandlerBuilderFilter>();
        host.Services.AddSingleton<IMessageHandler, MessageHandler>();
        host.Services.AddSingleton<WeatherFlowWebsocketClient>();
        host.Services.AddSingleton<IWebsocketClient, WebsocketClientWrapper>((CreateWebSocketClient));

        host.Services.AddLogging(x =>
        {
            x.AddConsole();
            x.AddFilter((category, level) => category != "Polly");
        });

        return host;

        WebsocketClientWrapper CreateWebSocketClient(IServiceProvider arg)
        {
            var token = host.Configuration.GetValue<string>("TOKEN");
            return new WebsocketClientWrapper(new Uri($"wss://ws.weatherflow.com/swd/data?token={token}"));
        }
    }
}