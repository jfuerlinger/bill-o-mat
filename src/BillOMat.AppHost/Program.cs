var builder = DistributedApplication.CreateBuilder(args);

var grafana = builder.AddContainer("grafana", "grafana/grafana")
                     .WithVolumeMount("../grafana/config", "/etc/grafana")
                     .WithVolumeMount("../grafana/dashboards", "/var/lib/grafana/dashboards")
                     .WithEndpoint(containerPort: 3000, hostPort: 3000, name: "grafana-http", scheme: "http");

var cache = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.BillOMat_Api>("apiservice")
    .WithEnvironment("GRAFANA_URL", grafana.GetEndpoint("grafana-http")); ;

builder.AddProject<Projects.BillOMat_BlazorUI>("webfrontend")
    .WithReference(cache)
    .WithReference(apiService);
    

builder.AddContainer("prometheus", "prom/prometheus")
       .WithVolumeMount("../prometheus", "/etc/prometheus")
       .WithEndpoint(9090, hostPort: 9090);


builder.Build().Run();
