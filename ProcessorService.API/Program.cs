using Hangfire;
using ProcessorService.Abstraction.Services;
using ProcessorService.Implementation;
using Serilog;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Hangfire.MemoryStorage;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Host.UseSerilog(logger);

builder.AddRedisClient(connectionName: "cache");

builder.Services.AddHangfire(config =>
{
    config.UseMemoryStorage(); // для демонстрации; в продакшене можно использовать Hangfire.PostgreSql
});
builder.Services.AddHangfireServer();

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddServiceDefaults();

builder.Services.AddScoped<IArbitrageJob, ArbitrageJob>();
builder.Services.AddScoped<IArbitrageCalculator, ArbitrageCalculator>();
builder.Services.AddSingleton<IDistributedCache, RedisCache>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseHangfireDashboard();

RecurringJob.AddOrUpdate<IArbitrageJob>(
    "ArbitrageJob",
    job => job.ExecuteAsync(),
    Cron.Minutely);

app.Run();
