using System.Net.Http.Json;
using System.Text.Json;
using HabitManager.DataModels;

namespace HabitManager.App.Services;

public class HabitApiClient
{
    private readonly HttpClient _httpClient;

    public HabitApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<HabitDto>> GetAllAsync()
    {
        try
        {
            using var response = await _httpClient.GetAsync("api/habits");

            await EnsureSuccessAsync(response);

            return await response.Content.ReadFromJsonAsync<List<HabitDto>>()
                ?? throw new InvalidOperationException(
                    "The server returned an empty response.");
        }
        catch (HttpRequestException exception)
        {
            throw new InvalidOperationException(
                "Cannot connect to the server. Check your connection and make sure the API is running.",
                exception);
        }
        catch (TaskCanceledException exception)
        {
            throw new InvalidOperationException(
                "The server did not respond in time. Please try again.",
                exception);
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException(
                "The server returned an invalid response.",
                exception);
        }
    }

    public async Task<HabitDto> CreateAsync(CreateHabitRequest request)
    {
        try
        {
            using var response = await _httpClient.PostAsJsonAsync(
                "api/habits",
                request);

            await EnsureSuccessAsync(response);

            return await response.Content.ReadFromJsonAsync<HabitDto>()
                   ?? throw new InvalidOperationException(
                       "The server returned an empty response.");
        }
        catch (HttpRequestException exception)
        {
            throw new InvalidOperationException(
                "Cannot connect to the server. Check your connection and make sure the API is running.",
                exception);
        }
        catch (TaskCanceledException exception)
        {
            throw new InvalidOperationException(
                "The server did not respond in time. Check the habit list before trying again.",
                exception);
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException(
                "The server returned an invalid response. Check the habit list before trying again.",
                exception);
        }
    }
    
    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        string message =
            $"The request failed (HTTP {(int)response.StatusCode}).";

        string content = await response.Content.ReadAsStringAsync();

        if (!string.IsNullOrWhiteSpace(content))
        {
            try
            {
                using var document = JsonDocument.Parse(content);
                var root = document.RootElement;

                if (root.ValueKind == JsonValueKind.Object)
                {
                    if (root.TryGetProperty("detail", out var detail) &&
                        detail.ValueKind == JsonValueKind.String &&
                        !string.IsNullOrWhiteSpace(detail.GetString()))
                    {
                        message = detail.GetString()!;
                    }
                    else if (root.TryGetProperty("title", out var title) &&
                             title.ValueKind == JsonValueKind.String &&
                             !string.IsNullOrWhiteSpace(title.GetString()))
                    {
                        message = title.GetString()!;
                    }
                }
            }
            catch (JsonException)
            {
                // Ja atbilde nav JSON, izmantojam HTTP kļūdas paziņojumu.
            }
        }

        throw new InvalidOperationException(message);
    }
    
    public async Task<HabitDto> UpdateAsync(UpdateHabitRequest request)
    {
        try
        {
            using var response = await _httpClient.PutAsJsonAsync(
                $"api/habits/{request.Id}",
                request);

            await EnsureSuccessAsync(response);

            return await response.Content.ReadFromJsonAsync<HabitDto>()
                   ?? throw new InvalidOperationException(
                       "The server returned an empty response.");
        }
        catch (HttpRequestException exception)
        {
            throw new InvalidOperationException(
                "Cannot connect to the server. Check your connection and make sure the API is running.",
                exception);
        }
        catch (TaskCanceledException exception)
        {
            throw new InvalidOperationException(
                "The server did not respond in time. Reload the habit to check whether the changes were saved.",
                exception);
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException(
                "The server returned an invalid response. Reload the habit to check whether the changes were saved.",
                exception);
        }
    }
    
    public Task CompleteAsync(Guid id, DateOnly date)
    {
        var request = new HabitCompletionRequest
        {
            HabitId = id,
            Date = date
        };

        return SendChangeAsync(() =>
            _httpClient.PostAsJsonAsync(
                $"api/habits/{id}/completions",
                request));
    }

    public Task UndoCompletionAsync(Guid id, DateOnly date)
    {
        string formattedDate = date.ToString(
            "yyyy-MM-dd",
            System.Globalization.CultureInfo.InvariantCulture);

        return SendChangeAsync(() =>
            _httpClient.DeleteAsync(
                $"api/habits/{id}/completions/{formattedDate}"));
    }

    public Task SetArchivedAsync(Guid id, bool isArchived)
    {
        string url = $"api/habits/{id}/archive";

        return SendChangeAsync(() => isArchived
            ? _httpClient.PutAsync(url, null)
            : _httpClient.DeleteAsync(url));
    }

    private async Task SendChangeAsync(
        Func<Task<HttpResponseMessage>> sendRequest)
    {
        try
        {
            using var response = await sendRequest();

            await EnsureSuccessAsync(response);
        }
        catch (HttpRequestException exception)
        {
            throw new InvalidOperationException(
                "Cannot connect to the server. Check your connection and make sure the API is running.",
                exception);
        }
        catch (TaskCanceledException exception)
        {
            throw new InvalidOperationException(
                "The server did not respond in time. Reload the habit to check whether the changes were saved.",
                exception);
        }
    }
}