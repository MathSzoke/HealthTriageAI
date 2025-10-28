namespace HealthTriageAI.ApiService.Services.Abstractions;

public interface IAdviceAgent
{
    Task<string> GenerateAsync(Enums.TriageLevel level, Enums.Specialist specialist);
}