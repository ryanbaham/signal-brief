using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SignalBrief.Api.Services;

namespace SignalBrief.Api.Functions;

public sealed class NewslettersFunction(NewsletterBlobStore newsletters)
{
    [Function("GetNewsletterIndex")]
    public async Task<HttpResponseData> GetNewsletterIndex(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "newsletters")] HttpRequestData request,
        CancellationToken cancellationToken)
    {
        return await WriteBlobJsonAsync(request, () => newsletters.GetIndexAsync(cancellationToken));
    }

    [Function("GetNewsletterIssue")]
    public async Task<HttpResponseData> GetNewsletterIssue(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "newsletters/{issueDate}")] HttpRequestData request,
        string issueDate,
        CancellationToken cancellationToken)
    {
        if (!NewsletterBlobStore.IsValidIssueDate(issueDate))
        {
            var invalid = request.CreateResponse(System.Net.HttpStatusCode.BadRequest);
            await invalid.WriteAsJsonAsync(new { error = "Issue dates must use yyyy-MM-dd format." }, cancellationToken);
            return invalid;
        }

        return await WriteBlobJsonAsync(request, () => newsletters.GetIssueAsync(issueDate, cancellationToken));
    }

    private static async Task<HttpResponseData> WriteBlobJsonAsync(
        HttpRequestData request,
        Func<Task<BlobJsonResult>> readJson)
    {
        try
        {
            var result = await readJson();
            var response = request.CreateResponse(System.Net.HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            response.Headers.Add("Cache-Control", result.CacheControl);
            await response.WriteStringAsync(result.Json);
            return response;
        }
        catch (BlobJsonNotFoundException exception)
        {
            var response = request.CreateResponse(System.Net.HttpStatusCode.NotFound);
            await response.WriteAsJsonAsync(new { error = exception.Message });
            return response;
        }
        catch (InvalidOperationException exception)
        {
            var response = request.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            await response.WriteAsJsonAsync(new { error = exception.Message });
            return response;
        }
    }
}
