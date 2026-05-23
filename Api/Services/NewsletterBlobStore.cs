using System.Globalization;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

namespace SignalBrief.Api.Services;

public sealed class NewsletterBlobStore
{
    private const string DefaultIndexBlobName = "newsletters/index.json";
    private const string DefaultIssueBlobNamePattern = "newsletters/{date}.json";

    private readonly BlobContainerClient _container;
    private readonly string _indexBlobName;
    private readonly string _issueBlobNamePattern;

    public NewsletterBlobStore(IConfiguration configuration)
    {
        var connectionString = configuration["Newsletter:BlobConnectionString"];
        var containerName = configuration["Newsletter:ContainerName"];

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Missing Newsletter__BlobConnectionString app setting.");
        }

        if (string.IsNullOrWhiteSpace(containerName))
        {
            throw new InvalidOperationException("Missing Newsletter__ContainerName app setting.");
        }

        _container = new BlobContainerClient(connectionString, containerName);
        _indexBlobName = configuration["Newsletter:IndexBlobName"] ?? DefaultIndexBlobName;
        _issueBlobNamePattern = configuration["Newsletter:IssueBlobNamePattern"] ?? DefaultIssueBlobNamePattern;
    }

    public Task<BlobJsonResult> GetIndexAsync(CancellationToken cancellationToken)
    {
        return DownloadJsonAsync(_indexBlobName, "public, max-age=60", cancellationToken);
    }

    public Task<BlobJsonResult> GetIssueAsync(string issueDate, CancellationToken cancellationToken)
    {
        var blobName = _issueBlobNamePattern.Replace("{date}", issueDate, StringComparison.OrdinalIgnoreCase);
        return DownloadJsonAsync(blobName, "public, max-age=300", cancellationToken);
    }

    public static bool IsValidIssueDate(string issueDate)
    {
        return DateOnly.TryParseExact(
            issueDate,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out _);
    }

    private async Task<BlobJsonResult> DownloadJsonAsync(
        string blobName,
        string cacheControl,
        CancellationToken cancellationToken)
    {
        try
        {
            var blob = _container.GetBlobClient(blobName);
            Response<BlobDownloadResult> download = await blob.DownloadContentAsync(cancellationToken);
            return new BlobJsonResult(download.Value.Content.ToString(), cacheControl);
        }
        catch (RequestFailedException exception) when (exception.Status == 404)
        {
            throw new BlobJsonNotFoundException($"Newsletter blob '{blobName}' was not found.");
        }
    }
}
