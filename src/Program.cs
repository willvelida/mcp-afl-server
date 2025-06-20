using System.Net.Http.Headers;
using mcp_afl_server.Tools;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

builder.Services.AddOpenTelemetry()
    .WithTracing(b => b.AddSource("*")
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation())
    .WithMetrics(b => b.AddMeter("*")
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation())
    .WithLogging()
    .UseOtlpExporter();

builder.Services.AddSingleton(_ =>
{
    var client = new HttpClient() { BaseAddress = new Uri("https://api.squiggle.com.au/") };
    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("mcp-afl-server", "1.0"));
    return client;
});

builder.Services.AddScoped<GameTools>();
builder.Services.AddScoped<LadderTools>();
builder.Services.AddScoped<PowerRankingsTools>();
builder.Services.AddScoped<SourcesTools>();
builder.Services.AddScoped<StandingsTools>();
builder.Services.AddScoped<TeamTools>();
builder.Services.AddScoped<TipsTools>();

var app = builder.Build();

app.MapGet("/api/healthz", () => Results.Ok("Healthy"));

app.MapMcp();

app.Run();