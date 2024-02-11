var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.BillOMat_Api>("apiservice");

builder.AddProject<Projects.BillOMat_BlazorUI>("webfrontend")
    .WithReference(cache)
    .WithReference(apiService);

builder.Build().Run();
