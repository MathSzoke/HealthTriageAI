using HealthTriageAI.ApiService.Hubs;
using HealthTriageAI.ApiService.Orchestration;
using HealthTriageAI.ApiService.Services;
using HealthTriageAI.ApiService.Services.Abstractions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var allowedOrigin = "https://healthtriage.mathszoke.com";

builder.Services.AddSignalR();

builder.Services.AddCors(o =>
{
    o.AddPolicy("Cors", p =>
        p.WithOrigins(allowedOrigin)
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HealthTriage API", Version = "v1" }));

builder.Services.AddSingleton<TriageCoordinator>();
builder.Services.AddTransient<SymptomAgent>();
builder.Services.AddTransient<RiskAgent>();
builder.Services.AddTransient<SpecialistAgent>();
builder.Services.AddTransient<IAdviceAgent, AdviceAgent>();
builder.Services.AddTransient<IFirstAidAgent, FirstAidAgent>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseCors("Cors");

app.MapHub<TriageHub>("/hubs/triage").RequireCors("Cors");

app.MapPost("/api/triage/report", async (TriageInput input, TriageCoordinator coord) =>
{
    var id = await coord.StartAsync(input);
    return Results.Ok(new { caseId = id });
}).RequireCors("Cors");

app.Run();

public record TriageInput(string Name, int Age, string Symptoms, double? Temperature, int? HeartRate, int? SystolicBP, string Location);
