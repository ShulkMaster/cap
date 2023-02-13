using Cap.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace Cap.Filter;

public class ApiKeyAuthFilter: IAsyncAuthorizationFilter
{
    private readonly IConfiguration _config;
    private readonly QuakeContext _db;
    public const string ApiKeyHeader = "CAP-API-KEY";
    
    public ApiKeyAuthFilter(IConfiguration config, QuakeContext db)
    {
        _config = config;
        _db = db;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        HttpContext http = context.HttpContext;
        IHeaderDictionary headers = http.Request.Headers;

        bool hasHeader = headers.TryGetValue(ApiKeyHeader, out StringValues values);

        if (!hasHeader) {
            context.Result = new UnauthorizedObjectResult("API key missing in headers");
            return;
        }
        
        string? apiKey = _config.GetValue<string>(ApiKeyHeader);
        if (!apiKey.Equals(values)) {
            context.Result = new UnauthorizedObjectResult("Unauthorized client");
        }
    }
}