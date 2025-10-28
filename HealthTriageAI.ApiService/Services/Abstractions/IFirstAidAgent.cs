namespace HealthTriageAI.ApiService.Services.Abstractions;

public interface IFirstAidAgent
{
    Task<string> GenerateAsync(string symptoms, double? temperature, int? heartRate, int? systolicBP, Enums.TriageLevel level);
}