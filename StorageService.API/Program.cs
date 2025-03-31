using Microsoft.EntityFrameworkCore;
using Serilog;
using StorageService.Abstraction.Repositories;
using StorageService.DAL;
using StorageService.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Host.UseSerilog(logger);

var connectionString = builder.Configuration.GetConnectionString("postgresdb");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IArbitrageDifferenceRepository, ArbitrageDifferenceRepository>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.AddServiceDefaults();

builder.AddNpgsqlDataSource("postgresdb");

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
