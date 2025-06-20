using System.Net.Http.Headers;
using mcp_afl_server.Tools;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

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

app.UseCors();

// Enhanced request logging
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    
    logger.LogInformation("Request: {Method} {Path} {QueryString}", 
        context.Request.Method, 
        context.Request.Path, 
        context.Request.QueryString);
    
    // Log request headers for SSE and message endpoints
    if (context.Request.Path.StartsWithSegments("/sse") || 
        context.Request.Path.StartsWithSegments("/message") ||
        context.Request.Path == "/")
    {
        logger.LogInformation("Headers: {Headers}", 
            string.Join(", ", context.Request.Headers.Select(h => $"{h.Key}={string.Join(",", h.Value.ToArray())}")));
    }
    
    await next();
    
    logger.LogInformation("Response: {StatusCode} for {Method} {Path}", 
        context.Response.StatusCode, 
        context.Request.Method, 
        context.Request.Path);
});

app.MapGet("/api/healthz", () => Results.Ok("Healthy"));

// Map MCP endpoints
app.MapMcp();

// Log all registered services (debug info)
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("=== Registered Services ===");
var serviceProvider = app.Services;
var services = serviceProvider.GetServices<object>();
foreach (var service in services.Take(10)) // Just show first 10 to avoid spam
{
    logger.LogInformation("Service: {ServiceType}", service.GetType().Name);
}

// Log all endpoints after mapping
logger.LogInformation("=== Registered Endpoints ===");
var endpointDataSource = app.Services.GetRequiredService<EndpointDataSource>();
foreach (var endpoint in endpointDataSource.Endpoints)
{
    if (endpoint is RouteEndpoint routeEndpoint)
    {
        var methods = routeEndpoint.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods ?? new[] { "ANY" };
        var pattern = routeEndpoint.RoutePattern.RawText ?? "Unknown";
        logger.LogInformation("Endpoint: {Methods} {Pattern} - {DisplayName}", 
            string.Join(", ", methods), 
            pattern,
            endpoint.DisplayName);
    }
    else
    {
        logger.LogInformation("Endpoint: {DisplayName}", endpoint.DisplayName ?? "Unknown");
    }
}

app.Run();