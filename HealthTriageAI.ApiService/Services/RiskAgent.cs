using HealthTriageAI.ApiService.Enums;

namespace HealthTriageAI.ApiService.Services;

public class RiskAgent
{
    public Task<TriageLevel> EvaluateAsync(Dictionary<string, double> metrics, int age)
    {
        var score = 0;
        if (metrics.TryGetValue("temperature", out var t) && t >= 39) score += 2;
        if (metrics.TryGetValue("heartrate", out var hr) && hr >= 120) score += 2;
        if (metrics.TryGetValue("systolic", out var sbp) && sbp <= 90) score += 3;
        if (metrics.ContainsKey("flag_chest_pain")) score += 3;
        if (metrics.ContainsKey("flag_dyspnea")) score += 2;
        if (metrics.ContainsKey("flag_syncope")) score += 2;
        if (age >= 70) score += 1;
        var level = score switch
        {
            <= 1 => TriageLevel.Low,
            <= 3 => TriageLevel.Moderate,
            <= 5 => TriageLevel.High,
            _ => TriageLevel.Critical
        };
        return Task.FromResult(level);
    }
}