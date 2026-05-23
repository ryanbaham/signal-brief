# Signal Brief

Signal Brief is a Blazor WebAssembly newsletter reader designed for Azure Static Web Apps. It renders newsletter issues from JSON stored in Azure Blob Storage, with the latest issue loaded by default and older issues available through date-based routes such as `/issue/2026-05-23`.

## Architecture

- `Client`: .NET 10 Blazor WebAssembly front end.
- `Api`: Azure Static Web Apps managed Functions API using .NET 9 isolated, which is the latest .NET isolated runtime currently listed for SWA managed APIs.
- `Shared`: newsletter contracts used by both projects.
- `Client/wwwroot/staticwebapp.config.json`: Static Web Apps navigation fallback, response headers, MIME types, and API runtime config.

The browser calls:

- `GET /api/newsletters` for the archive index.
- `GET /api/newsletters/{yyyy-MM-dd}` for a specific issue.

The API reads those JSON documents from Azure Blob Storage. This keeps the blob container private while the site itself remains open for anonymous readers.

## Blob JSON layout

Default blob names:

```text
newsletters/index.json
newsletters/{yyyy-MM-dd}.json
```

`index.json` drives the latest issue and pagination:

```json
{
  "latestDate": "2026-05-23",
  "issues": [
    {
      "date": "2026-05-23",
      "title": "The AI stack is moving from demo magic to operating discipline.",
      "editionLabel": "Weekly Tech Intelligence"
    }
  ]
}
```

Each issue JSON follows the shape in `reference/tech-newsletter.data.example.json`. Local sample files are included under `Client/wwwroot/newsletters` so the UI can render without Azure during development.

## Azure app settings

Set these on the Static Web App:

```text
Newsletter__BlobConnectionString=<storage connection string>
Newsletter__ContainerName=<container name>
Newsletter__IndexBlobName=newsletters/index.json
Newsletter__IssueBlobNamePattern=newsletters/{date}.json
```

Authentication is currently disabled. When it is time to re-enable social login, add the provider registrations back to `Client/wwwroot/staticwebapp.config.json` and protect `/api/newsletters*` with the `authenticated` role.

## Deployment

The GitHub Actions workflow publishes the Blazor client and Functions API before uploading the built outputs to Azure Static Web Apps. Add the deployment token as this repository secret:

```text
AZURE_STATIC_WEB_APPS_API_TOKEN
```

## Local development

Run the client:

```bash
dotnet run --project Client/BlazorBasic.csproj
```

The local app falls back to the sample JSON in `Client/wwwroot/newsletters` when the Azure-backed API is unavailable.

Build everything:

```bash
dotnet build BlazorBasic.sln
```
