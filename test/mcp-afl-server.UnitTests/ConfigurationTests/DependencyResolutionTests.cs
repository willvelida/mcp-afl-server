using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using mcp_afl_server.Tools;

namespace mcp_afl_server.UnitTests.ConfigurationTests
{
    public class DependencyResolutionTests
    {
        private ServiceProvider CreateFullServiceProvider()
        {
            var services = new ServiceCollection();
            
            // Add dependencies as in Program.cs
            services.AddLogging();
            services.AddHttpClient();
            services.AddSingleton<HttpClient>(provider => 
                provider.GetRequiredService<IHttpClientFactory>().CreateClient("SquiggleApi"));
            
            // Add all tool services
            services.AddScoped<GameTools>();
            services.AddScoped<LadderTools>();
            services.AddScoped<PowerRankingsTools>();
            services.AddScoped<SourcesTools>();
            services.AddScoped<StandingsTools>();
            services.AddScoped<TeamTools>();
            services.AddScoped<TipsTools>();
            
            return services.BuildServiceProvider();
        }

        [Theory]
        [InlineData(typeof(GameTools))]
        [InlineData(typeof(LadderTools))]
        [InlineData(typeof(PowerRankingsTools))]
        [InlineData(typeof(SourcesTools))]
        [InlineData(typeof(StandingsTools))]
        [InlineData(typeof(TeamTools))]
        [InlineData(typeof(TipsTools))]
        public void ToolService_CanBeResolved_WithoutExceptions(Type toolType)
        {
            // Arrange
            var serviceProvider = CreateFullServiceProvider();

            // Act & Assert
            using var scope = serviceProvider.CreateScope();
            Action resolve = () => scope.ServiceProvider.GetRequiredService(toolType);
            
            resolve.Should().NotThrow();
            var service = scope.ServiceProvider.GetRequiredService(toolType);
            service.Should().NotBeNull();
            service.Should().BeOfType(toolType);
        }

        [Fact]
        public void AllRequiredServices_CanBeResolved()
        {
            // Arrange
            var serviceProvider = CreateFullServiceProvider();

            // Act & Assert
            using var scope = serviceProvider.CreateScope();
            
            scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().Should().NotBeNull();
            scope.ServiceProvider.GetRequiredService<HttpClient>().Should().NotBeNull();
            scope.ServiceProvider.GetRequiredService<ILogger<GameTools>>().Should().NotBeNull();
        }
    }
}