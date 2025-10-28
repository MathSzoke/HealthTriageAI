var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");

var apiService = builder.AddProject<Projects.HealthTriageAI_ApiService>("apiservice")
    .WithReference(redis)
    .WithHttpHealthCheck("/health")
    .WaitFor(redis);

var frontend = builder.AddNpmApp("frontend", "../healthtriageai.web", "dev")
    .WithHttpEndpoint(5173, env: "VITE_PORT")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithEnvironment("VITE_API_BASE", apiService.GetEndpoint("https"));

builder.Build().Run();
