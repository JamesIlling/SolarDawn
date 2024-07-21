using System.Diagnostics.CodeAnalysis;
using SolarDawn.ServiceDefaults;

namespace SolarDawn.ApiService;
[ExcludeFromCodeCoverage]
public static class Program
{

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add service defaults & Aspire components.
        builder.AddServiceDefaults();

        // Add services to the container.
        builder.Services.AddProblemDetails();
        builder.Services.AddControllers();
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseExceptionHandler();

        app.MapControllers();

        app.MapDefaultEndpoints();


        app.Run();

    }
}