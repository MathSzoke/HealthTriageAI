using HealthTriageAI.ApiService.Enums;

namespace HealthTriageAI.ApiService.Services;

public class SpecialistAgent
{
    public Task<Specialist> DecideAsync(string symptoms, TriageLevel level)
    {
        if (symptoms.Contains("dor no peito", StringComparison.OrdinalIgnoreCase)) return Task.FromResult(Specialist.Cardiology);
        if (symptoms.Contains("falta de ar", StringComparison.OrdinalIgnoreCase)) return Task.FromResult(Specialist.Pulmonology);
        if (symptoms.Contains("febre", StringComparison.OrdinalIgnoreCase)) return Task.FromResult(Specialist.InfectiousDisease);
        if (level == TriageLevel.Critical) return Task.FromResult(Specialist.Emergency);
        return Task.FromResult(Specialist.GeneralPractice);
    }
}