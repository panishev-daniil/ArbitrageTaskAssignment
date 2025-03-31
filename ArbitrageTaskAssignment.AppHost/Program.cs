

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
                      .WithPgAdmin()
                      .WithLifetime(ContainerLifetime.Persistent);

var postgresdb = postgres.AddDatabase("postgresdb");

var cache = builder.AddRedis("cache")
                   .WithDataVolume()
                   .WithLifetime(ContainerLifetime.Persistent)
                   .WithPersistence(
                       interval: TimeSpan.FromMinutes(5),
                       keysChangedThreshold: 100);

var priceFetcher = builder.AddProject<Projects.PriceFetcherService_API>("pricefetcherapi");

var storageService = builder.AddProject<Projects.StorageService_API>("storageservice")
    .WithReference(cache)
    .WithReference(postgresdb);

var processorService = builder.AddProject<Projects.ProcessorService_API>("processorservice")
    .WithReference(cache)
    .WithReference(priceFetcher)
    .WithReference(storageService)
    .WithExternalHttpEndpoints();


builder.Build().Run();