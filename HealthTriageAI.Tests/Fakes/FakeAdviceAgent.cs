using HealthTriageAI.ApiService.Enums;
using HealthTriageAI.ApiService.Services.Abstractions;

namespace HealthTriageAI.Tests.Fakes;

public class FakeAdviceAgent : IAdviceAgent
{
    public Task<string> GenerateAsync(TriageLevel level, Specialist specialist)
        => Task.FromResult("fake-advice");
}
