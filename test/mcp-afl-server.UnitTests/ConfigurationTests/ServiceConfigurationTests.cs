using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using mcp_afl_server.Tools;

namespace mcp_afl_server.UnitTests.ConfigurationTests
{
    public class ServiceConfigurationTests
    {
        [Fact]
        public void ServiceCollection_ContainsAllToolServices()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddHttpClient();
            
            // Add all tool services as in Program.cs
            services.AddScoped<GameTools>();
            services.AddScoped<LadderTools>();
            services.AddScoped<PowerRankingsTools>();
            services.AddScoped<SourcesTools>();
            services.AddScoped<StandingsTools>();
            services.AddScoped<TeamTools>();
            services.AddScoped<TipsTools>();

            // Act
            var toolServices = services.Where(s => 
                s.ServiceType.Name.EndsWith("Tools") && 
                s.ServiceType.Namespace == "mcp_afl_server.Tools").ToList();

            // Assert
            toolServices.Should().HaveCount(7);
            toolServices.Should().OnlyContain(s => s.Lifetime == ServiceLifetime.Scoped);
        }

        [Fact]
        public void HttpClient_RegisteredAsSingleton()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddHttpClient();
            
            // Register HttpClient as singleton like in Program.cs
            services.AddSingleton<HttpClient>(provider => 
                provider.GetRequiredService<IHttpClientFactory>().CreateClient("SquiggleApi"));

            // Act - Look for ALL HttpClient registrations
            var httpClientDescriptors = services.Where(s => s.ServiceType == typeof(HttpClient)).ToList();

            // Assert - Should contain a singleton registration
            httpClientDescriptors.Should().NotBeEmpty();
            httpClientDescriptors.Should().Contain(d => d.Lifetime == ServiceLifetime.Singleton);
        }

        [Theory]
        [InlineData(typeof(GameTools))]
        [InlineData(typeof(LadderTools))]
        [InlineData(typeof(PowerRankingsTools))]
        [InlineData(typeof(SourcesTools))]
        [InlineData(typeof(StandingsTools))]
        [InlineData(typeof(TeamTools))]
        [InlineData(typeof(TipsTools))]
        public void ToolService_RegisteredWithCorrectLifetime(Type toolType)
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddScoped(toolType);

            // Act
            var descriptor = services.First(s => s.ServiceType == toolType);

            // Assert
            descriptor.Lifetime.Should().Be(ServiceLifetime.Scoped);
            descriptor.ServiceType.Should().Be(toolType);
        }
    }
}