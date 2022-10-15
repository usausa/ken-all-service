using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;

using KenAllService.Accessors;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Hosting.WindowsServices;

using Prometheus;

using Serilog;

using Smart.AspNetCore;
using Smart.AspNetCore.ApplicationModels;
using Smart.Data;
using Smart.Data.Accessor.Extensions.DependencyInjection;

#pragma warning disable CA1812

//--------------------------------------------------------------------------------
// Configure builder
//--------------------------------------------------------------------------------
Directory.SetCurrentDirectory(AppContext.BaseDirectory);

// Configure builder
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
});

// Service
builder.Host
    .UseWindowsService()
    .UseSystemd();

// Log
builder.Host
    .ConfigureLogging((_, logging) =>
    {
        logging.ClearProviders();
    })
    .UseSerilog((hostingContext, loggerConfiguration) =>
    {
        loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
    });

// Route
builder.Services.Configure<RouteOptions>(options =>
{
    options.AppendTrailingSlash = true;
});

// Filter
builder.Services.AddExceptionLogging();
builder.Services.AddTimeLogging(options =>
{
    options.Threshold = 5000;
});

// Add services to the container.
builder.Services
    .AddControllers(options =>
    {
        options.Filters.AddExceptionLogging();
        options.Filters.AddTimeLogging();
        options.Conventions.Add(new LowercaseControllerModelConvention());
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Health
builder.Services.AddHealthChecks();

// Swagger
builder.Services.AddSwaggerGen();

// Data
var connectionStringBuilder = new SqliteConnectionStringBuilder
{
    DataSource = "KenAll.db",
    Pooling = true,
    Cache = SqliteCacheMode.Shared
};
var connectionString = connectionStringBuilder.ConnectionString;

builder.Services.AddSingleton<IDbProvider>(new DelegateDbProvider(() => new SqliteConnection(connectionString)));
builder.Services.AddDataAccessor();

//--------------------------------------------------------------------------------
// Configure the HTTP request pipeline
//--------------------------------------------------------------------------------
var app = builder.Build();

// Prepare
if (!File.Exists(connectionStringBuilder.DataSource))
{
    var accessor = app.Services.GetRequiredService<IAddressAccessor>();
    accessor.Create();
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

// Health
app.UseHealthChecks("/health");

// Metrics
app.UseHttpMetrics();

// Map
app.MapMetrics();

app.MapControllers();
app.MapGet("/", async context => await context.Response.WriteAsync("KEN All API Service"));

// Run
app.Run();
