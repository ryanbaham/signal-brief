using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.JSInterop;

namespace SignalBrief.Client.Services;

public sealed class DeploymentInfoService(HttpClient httpClient, IJSRuntime jsRuntime)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<DeploymentDisplayInfo?> GetDisplayInfoAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var deployment = await httpClient.GetFromJsonAsync<DeploymentInfo>(
                "deployment.json",
                JsonOptions,
                cancellationToken);

            if (deployment?.DeployedAtUtc is null)
            {
                return null;
            }

            var deployedAtIso = deployment.DeployedAtUtc.Value.UtcDateTime.ToString("O", CultureInfo.InvariantCulture);
            var displayDate = await jsRuntime.InvokeAsync<string>(
                "signalBriefDeployment.formatDate",
                cancellationToken,
                deployedAtIso);
            var localTime = await jsRuntime.InvokeAsync<string>(
                "signalBriefDeployment.formatLocalTime",
                cancellationToken,
                deployedAtIso);

            return new DeploymentDisplayInfo(displayDate, localTime);
        }
        catch (Exception)
        {
            return null;
        }
    }
}

public sealed class DeploymentInfo
{
    public DateTimeOffset? DeployedAtUtc { get; set; }
    public string? CommitSha { get; set; }
}

public sealed record DeploymentDisplayInfo(string DisplayDate, string LocalTime);
