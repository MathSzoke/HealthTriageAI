using System.Reflection;
using HealthTriageAI.ApiService.Hubs;
using HealthTriageAI.ApiService.Orchestration;
using HealthTriageAI.ApiService.Services;
using HealthTriageAI.ApiService.Services.Abstractions;
using HealthTriageAI.Tests.Fakes;

namespace HealthTriageAI.Tests;

public class TriageCoordinatorTests
{
    private static string StepOf(object? arg)
        => arg == null ? "" : arg.GetType().GetProperty("step", BindingFlags.Public | BindingFlags.Instance)?.GetValue(arg)?.ToString() ?? "";

    private static string PayloadOf(object? arg)
        => arg == null ? "" : arg.GetType().GetProperty("payload", BindingFlags.Public | BindingFlags.Instance)?.GetValue(arg)?.ToString() ?? "";

    [Fact]
    public async Task StartAsync_RunsFullPipeline_EmitsAdviceAndFirstAid()
    {
        var symptom = new SymptomAgent();
        var risk = new RiskAgent();
        var spec = new SpecialistAgent();
        IAdviceAgent advice = new FakeAdviceAgent();
        IFirstAidAgent firstAid = new FakeFirstAidAgent();
        var hub = new CapturingHubContext<TriageHub>();

        var coord = new TriageCoordinator(symptom, risk, spec, advice, firstAid, hub);

        var input = new TriageInput("Test User", 64, "chest pain and shortness of breath", 39.2, 110, 95, "SP");
        var id = await coord.StartAsync(input);

        Assert.False(string.IsNullOrWhiteSpace(id));

        var calls = ((CapturingHubContext<TriageHub>.CapturingHubClients)hub.Clients).AllProxy.Calls.ToList();
        Assert.Contains(calls, c => c.Method == "case_step" && StepOf(c.Args[0]) == "first_aid");
        Assert.Contains(calls, c => c.Method == "case_step" && StepOf(c.Args[0]) == "advice");
        Assert.Contains(calls, c => c.Method == "case_updated" && c.Args.Any(a => a?.ToString()?.Contains("advice_ready") == true));
    }

    [Fact]
    public async Task StartAsync_EmitsStepsInOrder_SymptomRiskSpecialistFirstAidAdvice()
    {
        var symptom = new SymptomAgent();
        var risk = new RiskAgent();
        var spec = new SpecialistAgent();
        IAdviceAgent advice = new FakeAdviceAgent();
        IFirstAidAgent firstAid = new FakeFirstAidAgent();
        var hub = new CapturingHubContext<TriageHub>();

        var coord = new TriageCoordinator(symptom, risk, spec, advice, firstAid, hub);

        var input = new TriageInput("Order Test", 30, "fever and headache", 38.5, 88, 120, "RJ");
        await coord.StartAsync(input);

        var calls = ((CapturingHubContext<TriageHub>.CapturingHubClients)hub.Clients).AllProxy.Calls
            .OrderBy(c => c.Seq)
            .Where(c => c.Method == "case_step")
            .Select(c => StepOf(c.Args[0]))
            .ToList();

        var idxSymptom = calls.FindIndex(s => s == "symptom");
        var idxRisk = calls.FindIndex(s => s == "risk");
        var idxSpec = calls.FindIndex(s => s == "specialist");
        var idxFirstAid = calls.FindIndex(s => s == "first_aid");
        var idxAdvice = calls.FindIndex(s => s == "advice");

        Assert.True(idxSymptom >= 0, $"symptom is not first. steps: {string.Join(",", calls)}");
        Assert.True(idxRisk > idxSymptom, $"risk not after symptom. steps: {string.Join(",", calls)}");
        Assert.True(idxSpec > idxRisk, $"specialist not after risk. steps: {string.Join(",", calls)}");
        Assert.True(idxFirstAid > idxSpec, $"first_aid not after specialist. steps: {string.Join(",", calls)}");
        Assert.True(idxAdvice > idxFirstAid, $"advice not after first_aid. steps: {string.Join(",", calls)}");
    }

    [Fact]
    public async Task StartAsync_HandlesNullVitals_AndStillCompletesFlow()
    {
        var symptom = new SymptomAgent();
        var risk = new RiskAgent();
        var spec = new SpecialistAgent();
        IAdviceAgent advice = new FakeAdviceAgent();
        IFirstAidAgent firstAid = new FakeFirstAidAgent();
        var hub = new CapturingHubContext<TriageHub>();

        var coord = new TriageCoordinator(symptom, risk, spec, advice, firstAid, hub);

        var input = new TriageInput("Null Vitals", 22, "sore throat and low-grade fever", null, null, null, "SP");
        var id = await coord.StartAsync(input);

        Assert.False(string.IsNullOrWhiteSpace(id));

        var callSteps = ((CapturingHubContext<TriageHub>.CapturingHubClients)hub.Clients).AllProxy.Calls
            .Where(c => c.Method == "case_step")
            .Select(c => StepOf(c.Args[0]))
            .ToList();

        Assert.Contains("symptom", callSteps);
        Assert.Contains("risk", callSteps);
        Assert.Contains("first_aid", callSteps);
        Assert.Contains("advice", callSteps);
    }

    [Fact]
    public async Task StartAsync_SpecialistReflectsSymptoms_NotCriticalFallback()
    {
        var symptom = new SymptomAgent();
        var risk = new RiskAgent();
        var spec = new SpecialistAgent();
        IAdviceAgent advice = new FakeAdviceAgent();
        IFirstAidAgent firstAid = new FakeFirstAidAgent();
        var hub = new CapturingHubContext<TriageHub>();

        var coord = new TriageCoordinator(symptom, risk, spec, advice, firstAid, hub);

        var input = new TriageInput("Chest Case", 70, "dor no peito", 37.3, 72, 130, "SP");
        await coord.StartAsync(input);

        var specPayload = ((CapturingHubContext<TriageHub>.CapturingHubClients)hub.Clients).AllProxy.Calls
            .Where(c => c.Method == "case_step")
            .Select(c => c.Args[0])
            .FirstOrDefault(a => StepOf(a) == "specialist");

        Assert.NotNull(specPayload);
        Assert.Equal("Cardiology", PayloadOf(specPayload));
    }
}
