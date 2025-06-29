using System.Net.Http.Headers;
using mcp_afl_server.Configuration;
using mcp_afl_server.Services;
using mcp_afl_server.Tools;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add CORS for Azure Container Apps
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.Configure<AzureAdOptions>(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddMcpServer()
    .WithHttpTransport(options =>
    {
        options.Stateless = true;
    })
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

builder.Services.AddHttpClient<HttpClient>("SquiggleApi", client =>
{
    client.BaseAddress = new Uri("https://api.squiggle.com.au/");
    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("mcp-afl-server", "1.0"));
})
.AddStandardResilienceHandler();

builder.Services.AddSingleton<HttpClient>(provider => provider.GetRequiredService<IHttpClientFactory>().CreateClient("SquiggleApi"));

builder.Services.AddScoped<GameTools>();
builder.Services.AddScoped<LadderTools>();
builder.Services.AddScoped<PowerRankingsTools>();
builder.Services.AddScoped<SourcesTools>();
builder.Services.AddScoped<StandingsTools>();
builder.Services.AddScoped<TeamTools>();
builder.Services.AddScoped<TipsTools>();

var app = builder.Build();

app.UseCors();

app.MapGet("/api/healthz", () => Results.Ok("Healthy"));

// Map MCP endpoints
app.MapMcp();

app.Run();