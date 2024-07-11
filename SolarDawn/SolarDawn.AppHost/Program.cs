using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.SolarDawn_ApiService>("apiservice")
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.SolarDawn_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.AddProject<Projects.SolarDawn_TempestReader>("tempest-reader")
    .WithHttpsEndpoint()
    .WithReference(apiService)
    .WithEnvironment("TOKEN", "7cda791a-a7d4-4b5f-9d27-8ecfc0e57a74")
    .WithEnvironment("DEVICE_ID", "354105")
    .WithEnvironment("STATION_ID", "145787");

builder.Build().Run();
