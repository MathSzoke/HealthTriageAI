using HealthTriageAI.ApiService.Enums;

namespace HealthTriageAI.ApiService.Services;

public class SymptomAgent
{
    public Task<Dictionary<string, double>> AnalyzeAsync(string symptoms, double? temperature, int? heartRate, int? systolicBp)
    {
        var result = new Dictionary<string, double>();
        if (temperature.HasValue) result["temperature"] = temperature.Value;
        if (heartRate.HasValue) result["heartrate"] = heartRate.Value;
        if (systolicBp.HasValue) result["systolic"] = systolicBp.Value;
        if (!string.IsNullOrWhiteSpace(symptoms) && symptoms.Contains("dor no peito", StringComparison.OrdinalIgnoreCase)) result["flag_chest_pain"] = 1;
        if (!string.IsNullOrWhiteSpace(symptoms) && symptoms.Contains("falta de ar", StringComparison.OrdinalIgnoreCase)) result["flag_dyspnea"] = 1;
        if (!string.IsNullOrWhiteSpace(symptoms) && symptoms.Contains("desmaio", StringComparison.OrdinalIgnoreCase)) result["flag_syncope"] = 1;
        if (!string.IsNullOrWhiteSpace(symptoms) && symptoms.Contains("febre", StringComparison.OrdinalIgnoreCase)) result["flag_fever"] = 1;
        return Task.FromResult(result);
    }
}
