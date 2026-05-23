using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using SignalBrief.Shared.Newsletters;

namespace SignalBrief.Client.Services;

public sealed class NewsletterDataService(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<NewsletterIndex> GetIndexAsync(CancellationToken cancellationToken = default)
    {
        return await GetJsonWithStaticFallbackAsync<NewsletterIndex>(
            "api/newsletters",
            "newsletters/index.json",
            cancellationToken) ?? new NewsletterIndex();
    }

    public Task<NewsletterIssue?> GetIssueAsync(string issueDate, CancellationToken cancellationToken = default)
    {
        return GetJsonWithStaticFallbackAsync<NewsletterIssue>(
            $"api/newsletters/{Uri.EscapeDataString(issueDate)}",
            $"newsletters/{Uri.EscapeDataString(issueDate)}.json",
            cancellationToken);
    }

    private async Task<T?> GetJsonWithStaticFallbackAsync<T>(
        string apiPath,
        string staticPath,
        CancellationToken cancellationToken)
    {
        try
        {
            var apiResult = await GetJsonOrDefaultAsync<T>(apiPath, cancellationToken);

            if (apiResult is not null)
            {
                return apiResult;
            }
        }
        catch (HttpRequestException)
        {
        }
        catch (JsonException)
        {
        }

        return await httpClient.GetFromJsonAsync<T>(staticPath, JsonOptions, cancellationToken);
    }

    private async Task<T?> GetJsonOrDefaultAsync<T>(string path, CancellationToken cancellationToken)
    {
        using var response = await httpClient.GetAsync(path, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            if (response.Content.Headers.ContentType?.MediaType?.Contains("json", StringComparison.OrdinalIgnoreCase) != true)
            {
                return default;
            }

            return await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
        }

        if (response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
        {
            return default;
        }

        response.EnsureSuccessStatusCode();
        return default;
    }
}
