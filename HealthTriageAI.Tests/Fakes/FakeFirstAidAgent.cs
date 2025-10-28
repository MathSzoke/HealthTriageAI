using HealthTriageAI.ApiService.Enums;
using HealthTriageAI.ApiService.Services.Abstractions;

namespace HealthTriageAI.Tests.Fakes;

public class FakeFirstAidAgent : IFirstAidAgent
{
    public Task<string> GenerateAsync(string symptoms, double? temperature, int? heartRate, int? systolicBP, TriageLevel level)
        => Task.FromResult("fake-first-aid");
}
