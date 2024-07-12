namespace SolarDawn.AppHost
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = DistributedApplication.CreateBuilder(args);

            var apiService = builder.AddProject<Projects.SolarDawn_ApiService>("apiservice")
                .WithExternalHttpEndpoints();

            builder.AddProject<Projects.SolarDawn_Web>("webfrontend")
                .WithExternalHttpEndpoints()
                .WithReference(apiService);

            builder.AddProject<Projects.SolarDawn_TempestReader>("tempest-reader")
                .WithHttpsEndpoint()
                .WithReference(apiService)
                .WithEnvironment("TOKEN", builder.Configuration["tempest-token"])
                .WithEnvironment("DEVICE_ID", builder.Configuration["tempest-device-id"])
                .WithEnvironment("STATION_ID", builder.Configuration["tempest-station-id"]);

            await builder.Build().RunAsync();
        }
    }
}