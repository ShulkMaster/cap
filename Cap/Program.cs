using System.Text.Json.Serialization;
using Cap.Data;
using Cap.Filter;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NetTopologySuite.IO.Converters;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.Converters.Add(new GeoJsonConverterFactory());
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Logging.AddConsole();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<ApiKeyAuthFilter>();

builder.Services.AddCors(cors =>
{
    cors.AddDefaultPolicy(opts => opts.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CAP API",
        Version = "v1",
        Description = "Return information about earthquakes",
    });
    
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "Enter the API key:",
        Name = ApiKeyAuthFilter.ApiKeyHeader,
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey",
                },
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<QuakeContext>(opts =>
{
    string? config = builder.Configuration.GetConnectionString("prod");
    opts.UseNpgsql(config, conf =>
    {
        conf.CommandTimeout(60)
            .UseNetTopologySuite();
    });
    if (builder.Environment.IsDevelopment())
    {
        opts.EnableSensitiveDataLogging()
            .EnableDetailedErrors();
    }
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseCors();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();