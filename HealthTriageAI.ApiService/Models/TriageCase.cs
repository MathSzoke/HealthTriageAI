using HealthTriageAI.ApiService.Enums;

namespace HealthTriageAI.ApiService.Models;

public record TriageCase(
    string Id,
    string Name,
    int Age,
    string Symptoms,
    double? Temperature,
    int? HeartRate,
    int? SystolicBP,
    string Location,
    TriageLevel? Level,
    Specialist? Specialist,
    string Advice,
    string FirstAid,
    string Status,
    DateTimeOffset CreatedAt);