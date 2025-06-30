using mcp_afl_server.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using System.Collections;
using System.Net.Http.Json;
using System.Text.Json;

// Create a base class for all AFL tools
public abstract class BaseAFLTool
{
    protected readonly HttpClient _httpClient;
    protected readonly ILogger _logger;
    protected readonly IAuthenticationService _authenticationService;

    protected BaseAFLTool(
        HttpClient httpClient,
        ILogger logger,
        IAuthenticationService authenticationService)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
    }

    /// <summary>
    /// Validates authentication and gets the current user
    /// </summary>
    /// <returns>The authenticated user</returns>
    protected async Task<User> GetCurrentUserAsync()
    {
        return await _authenticationService.GetCurrentUserAsync();
    }

    /// <summary>
    /// Checks if the current request is authenticated
    /// </summary>
    /// <returns>True if authenticated</returns>
    protected bool IsAuthenticated()
    {
        return _authenticationService.IsAuthenticated();
    }

    /// <summary>
    /// Executes an API call with standardized error handling and logging
    /// </summary>
    /// <typeparam name="T">The type to deserialize the result to</typeparam>
    /// <param name="endpoint">The API endpoint to call</param>
    /// <param name="operationName">A descriptive name for logging purposes</param>
    /// <param name="propertyName">The JSON property name to extract from the response</param>
    /// <param name="additionalValidation">Optional additional validation function</param>
    /// <returns>The deserialized result or default(T) on error</returns>
    protected async Task<T> ExecuteApiCallAsync<T>(
        string endpoint,
        string operationName,
        string propertyName,
        Func<T, bool>? additionalValidation = null) where T : class, new()
    {
        _logger.LogInformation($"Fetching data for {operationName} - Endpoint: {endpoint}");

        try
        {
            var response = await _httpClient.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"API request failed. StatusCode: {response.StatusCode}, Endpoint: {endpoint}");
                return new T();
            }

            var jsonElement = await response.Content.ReadFromJsonAsync<JsonElement>();

            if (!jsonElement.TryGetProperty(propertyName, out var property))
            {
                _logger.LogWarning($"No '{propertyName}' property found in API response for {operationName}");
                return new T();
            }

            var result = JsonSerializer.Deserialize<T>(property.GetRawText());

            if (result == null)
            {
                _logger.LogWarning($"Failed to deserialize response for {operationName}");
                return new T();
            }

            // Check if result is a collection and log appropriately
            if (result is ICollection collection)
            {
                if (collection.Count == 0)
                {
                    _logger.LogInformation($"No data found for {operationName}");
                }
                else
                {
                    _logger.LogInformation($"Successfully retrieved {collection.Count} items for {operationName}");
                }
            }
            else
            {
                _logger.LogInformation($"Successfully retrieved data for {operationName}");
            }

            // Run additional validation if provided
            if (additionalValidation != null && !additionalValidation(result))
            {
                _logger.LogWarning($"Additional validation failed for {operationName}");
                return new T();
            }

            return result;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError($"Network error for {operationName}: {ex.Message}");
            return new T();
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError($"Timeout for {operationName}: {ex.Message}");
            return new T();
        }
        catch (JsonException ex)
        {
            _logger.LogError($"JSON parsing error for {operationName}: {ex.Message}");
            return new T();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unexpected error for {operationName}: {ex.Message}");
            return new T();
        }
    }

    /// <summary>
    /// Validates input parameters and logs warnings for invalid values
    /// </summary>
    protected bool ValidateParameters(params (string name, object value, Func<object, bool> validator, string errorMessage)[] validations)
    {
        foreach (var (name, value, validator, errorMessage) in validations)
        {
            if (!validator(value))
            {
                _logger.LogWarning($"Invalid {name} parameter: {value}. {errorMessage}");
                return false;
            }
        }
        return true;
    }

    // Common validation methods
    protected static bool IsValidYear(int year) => year >= 1897 && year <= DateTime.Now.Year + 1;
    protected static bool IsValidRound(int round) => round >= 1 && round <= 30;
    protected static bool IsValidId(int id) => id > 0;
    protected static bool IsValidString(string value) => !string.IsNullOrWhiteSpace(value);
}