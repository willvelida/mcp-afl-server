using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using System.Net.Http.Headers;

namespace mcp_afl_server.UnitTests.ConfigurationTests
{
    public class HttpClientConfigurationTests
    {
        [Fact]
        public void SquiggleApiHttpClient_HasCorrectBaseAddress()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddHttpClient<HttpClient>("SquiggleApi", client =>
            {
                client.BaseAddress = new Uri("https://api.squiggle.com.au/");
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("mcp-afl-server", "1.0"));
            });

            var serviceProvider = services.BuildServiceProvider();
            var factory = serviceProvider.GetRequiredService<IHttpClientFactory>();

            // Act
            var client = factory.CreateClient("SquiggleApi");

            // Assert
            client.BaseAddress.Should().Be("https://api.squiggle.com.au/");
        }

        [Fact]
        public void SquiggleApiHttpClient_HasCorrectUserAgent()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddHttpClient<HttpClient>("SquiggleApi", client =>
            {
                client.BaseAddress = new Uri("https://api.squiggle.com.au/");
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("mcp-afl-server", "1.0"));
            });

            var serviceProvider = services.BuildServiceProvider();
            var factory = serviceProvider.GetRequiredService<IHttpClientFactory>();

            // Act
            var client = factory.CreateClient("SquiggleApi");

            // Assert
            client.DefaultRequestHeaders.UserAgent.Should().ContainSingle(
                ua => ua.Product != null && ua.Product.Name == "mcp-afl-server" && ua.Product.Version == "1.0"
            );
        }

        [Fact]
        public void SquiggleApiHttpClient_UserAgentHeaderFormat_IsCorrect()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddHttpClient<HttpClient>("SquiggleApi", client =>
            {
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("mcp-afl-server", "1.0"));
            });

            var serviceProvider = services.BuildServiceProvider();
            var factory = serviceProvider.GetRequiredService<IHttpClientFactory>();

            // Act
            var client = factory.CreateClient("SquiggleApi");
            var userAgentString = client.DefaultRequestHeaders.UserAgent.ToString();

            // Assert
            userAgentString.Should().Be("mcp-afl-server/1.0");
        }

        [Fact]
        public void HttpClientFactory_IsRegistered()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddHttpClient();

            var serviceProvider = services.BuildServiceProvider();

            // Act
            var factory = serviceProvider.GetService<IHttpClientFactory>();

            // Assert
            factory.Should().NotBeNull();
        }

        [Fact]
        public void NamedHttpClient_CanBeCreatedMultipleTimes()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddHttpClient<HttpClient>("SquiggleApi", client =>
            {
                client.BaseAddress = new Uri("https://api.squiggle.com.au/");
            });

            var serviceProvider = services.BuildServiceProvider();
            var factory = serviceProvider.GetRequiredService<IHttpClientFactory>();

            // Act
            var client1 = factory.CreateClient("SquiggleApi");
            var client2 = factory.CreateClient("SquiggleApi");

            // Assert
            client1.Should().NotBeNull();
            client2.Should().NotBeNull();
            // Named clients should be different instances
            client1.Should().NotBeSameAs(client2);
            // But should have same configuration
            client1.BaseAddress.Should().Be(client2.BaseAddress);
        }

        [Fact]
        public void HttpClient_WithStandardResilienceHandler_CanBeCreated()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddHttpClient<HttpClient>("SquiggleApi", client =>
            {
                client.BaseAddress = new Uri("https://api.squiggle.com.au/");
            })
            .AddStandardResilienceHandler();

            var serviceProvider = services.BuildServiceProvider();
            var factory = serviceProvider.GetRequiredService<IHttpClientFactory>();

            // Act
            var client = factory.CreateClient("SquiggleApi");

            // Assert
            client.Should().NotBeNull();
            client.BaseAddress.Should().Be("https://api.squiggle.com.au/");
            // The resilience handler is internal, but we can verify the client was created successfully
        }
    }
}