using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using mcp_afl_server.Tools;

namespace mcp_afl_server.UnitTests.ConfigurationTests
{
    public class ServiceLifetimeTests
    {
        private ServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddHttpClient();
            services.AddScoped<GameTools>();
            services.AddSingleton<HttpClient>(provider => 
                provider.GetRequiredService<IHttpClientFactory>().CreateClient("TestClient"));
            
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
        public void ToolService_RegisteredAsScoped_HasCorrectLifetime(Type toolType)
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddHttpClient();
            services.AddScoped(toolType);

            // Act
            var descriptor = services.First(s => s.ServiceType == toolType);

            // Assert
            descriptor.Lifetime.Should().Be(ServiceLifetime.Scoped);
        }

        [Fact]
        public void ScopedServices_InSameScope_ReturnSameInstance()
        {
            // Arrange
            var serviceProvider = CreateServiceProvider();

            // Act
            using var scope = serviceProvider.CreateScope();
            var instance1 = scope.ServiceProvider.GetRequiredService<GameTools>();
            var instance2 = scope.ServiceProvider.GetRequiredService<GameTools>();

            // Assert
            instance1.Should().BeSameAs(instance2);
        }

        [Fact]
        public void ScopedServices_InDifferentScopes_ReturnDifferentInstances()
        {
            // Arrange
            var serviceProvider = CreateServiceProvider();

            // Act
            GameTools instance1, instance2;
            using (var scope1 = serviceProvider.CreateScope())
            {
                instance1 = scope1.ServiceProvider.GetRequiredService<GameTools>();
            }
            using (var scope2 = serviceProvider.CreateScope())
            {
                instance2 = scope2.ServiceProvider.GetRequiredService<GameTools>();
            }

            // Assert
            instance1.Should().NotBeSameAs(instance2);
        }

        [Fact]
        public void HttpClient_RegisteredAsSingleton_AlwaysReturnsSameInstance()
        {
            // Arrange
            var serviceProvider = CreateServiceProvider();

            // Act
            var client1 = serviceProvider.GetRequiredService<HttpClient>();
            var client2 = serviceProvider.GetRequiredService<HttpClient>();

            using var scope = serviceProvider.CreateScope();
            var client3 = scope.ServiceProvider.GetRequiredService<HttpClient>();

            // Assert
            client1.Should().BeSameAs(client2);
            client1.Should().BeSameAs(client3);
        }
    }
}