using PriceFetcherService.Abstraction.Services;
using PriceFetcherService.Implementation;
using Serilog;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

var elasticUri = builder.Configuration["ElasticConfiguration:Uri"];

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Host.UseSerilog(logger);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddServiceDefaults();

builder.Services.AddScoped<IPriceFetcher, PriceFetcher>();

builder.Services.AddHttpClient<PriceFetcher>(c =>
{
    var url = builder.Configuration["FetcherApi:BaseUrl"];
    c.BaseAddress = new(url);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
