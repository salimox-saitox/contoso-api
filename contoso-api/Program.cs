var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

var app = builder.Build();

var logger = app.Logger;

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var appEnv = Environment.GetEnvironmentVariable("APP_ENV") ?? "-";
var logLevel = Environment.GetEnvironmentVariable("LOG_LEVEL") ?? "-";
var apiVersion = Environment.GetEnvironmentVariable("API_VERSION") ?? "-";
var apiKey = Environment.GetEnvironmentVariable("API_KEY") ?? "-";
var backendUrl = Environment.GetEnvironmentVariable("BACKEND_URL") ?? "-";

app.MapGet("/companies", (HttpRequest request) =>
{
    logger.LogInformation("GET /companies - Request from {Host}{Path}{Query}",
        request.Host, request.Path, request.QueryString);
    var companies = new[]
    {
        new Company(1, "Contoso Ltd", "Seattle", "Technology"),
        new Company(2, "Fabrikam Inc", "San Francisco", "Manufacturing"),
        new Company(3, "Northwind Traders", "Chicago", "Retail"),
        new Company(4, "Adventure Works", "Dallas", "Tourism"),
        new Company(5, "Tailspin Toys", "New York", "Entertainment")
    };
    logger.LogInformation("GET /companies - Response: {Count} companies returned", companies.Length);
    return Results.Ok(companies);
})
.WithName("GetCompanies")
.WithOpenApi();

app.MapGet("/v2/companies", (HttpRequest request) =>
{
    logger.LogInformation("GET /v2/companies - Request from {Host}{Path}{Query}",
        request.Host, request.Path, request.QueryString);
    var companies = new[]
    {
        new Company(1, "Contoso Ltd", "Seattle", "Technology"),
        new Company(2, "Fabrikam Inc", "San Francisco", "Manufacturing"),
        new Company(3, "Northwind Traders", "Chicago", "Retail"),
        new Company(4, "Adventure Works", "Dallas", "Tourism"),
        new Company(5, "Tailspin Toys", "New York", "Entertainment")
    };
    logger.LogInformation("GET /v2/companies - Response: {Count} companies returned", companies.Length);
    return Results.Ok(companies);
})
.WithName("GetCompaniesV2")
.WithOpenApi();

app.MapGet("/config", (HttpRequest request) =>
{
    logger.LogInformation("GET /config - Request from {Host}{Path}{Query}",
        request.Host, request.Path, request.QueryString);
    var config = new
    {
        Environment = appEnv,
        LogLevel = logLevel,
        ApiVersion = apiVersion,
        ApiKey = apiKey
    };
    logger.LogInformation("GET /config - Response: Env={Env}, LogLevel={LogLevel}, ApiVersion={ApiVersion}",
        appEnv, logLevel, apiVersion);
    return Results.Ok(config);
})
.WithName("GetConfig")
.WithOpenApi();

app.MapGet("/backend", async (HttpRequest request, IHttpClientFactory httpClientFactory) =>
{
    logger.LogInformation("GET /backend - Request from {Host}{Path}{Query}",
        request.Host, request.Path, request.QueryString);

    if (backendUrl == "-")
    {
        logger.LogWarning("GET /backend - BACKEND_URL not configured");
        return Results.BadRequest("BACKEND_URL not configured");
    }

    var client = httpClientFactory.CreateClient();
    var response = await client.GetStringAsync(backendUrl);
    logger.LogInformation("GET /backend - Response received from {Url}", backendUrl);
    return Results.Ok(response);
})
.WithName("GetBackend")
.WithOpenApi();

app.Run();

record Company(int Id, string Name, string City, string Industry);
