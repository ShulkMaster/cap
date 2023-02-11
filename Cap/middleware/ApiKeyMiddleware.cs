using Microsoft.Extensions.Primitives;

namespace Cap.middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    public const string ApiKeyHeader = "CAP-API-KEY";
    
    public ApiKeyMiddleware(RequestDelegate next) {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        IHeaderDictionary headers = context.Request.Headers;

        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }
        
        bool hasHeader = headers.TryGetValue(ApiKeyHeader, out StringValues values);
        foreach ((string? key, StringValues value) in headers)
        {
            Console.WriteLine($"{key}: {value}");
        }
        
        if (!hasHeader) {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Api Key was not provided");
            return;
        }

        var appSettings = context.RequestServices.GetRequiredService<IConfiguration>();
        string? apiKey = appSettings.GetValue<string>(ApiKeyHeader);
        if (!apiKey.Equals(values)) {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized client");
            return;
        }
        await _next(context);
    }
}