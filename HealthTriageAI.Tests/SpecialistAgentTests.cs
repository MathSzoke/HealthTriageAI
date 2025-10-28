using HealthTriageAI.ApiService.Enums;
using HealthTriageAI.ApiService.Services;
using Xunit;

namespace HealthTriageAI.Tests;

public class SpecialistAgentTests
{
    [Theory]
    [InlineData("dor no peito persistente", Specialist.Cardiology)]
    [InlineData("falta de ar ao esforço", Specialist.Pulmonology)]
    [InlineData("febre alta e calafrios", Specialist.InfectiousDisease)]
    public async Task DecideAsync_MapsSymptomsToSpecialist(string symptoms, Specialist expected)
    {
        var agent = new SpecialistAgent();
        var level = TriageLevel.Moderate;
        var spec = await agent.DecideAsync(symptoms, level);
        Assert.Equal(expected, spec);
    }

    [Fact]
    public async Task DecideAsync_DefaultsToGeneralPractice_OnLowLevelAndNeutralSymptoms()
    {
        var agent = new SpecialistAgent();
        var spec = await agent.DecideAsync("cansaço leve", TriageLevel.Low);
        Assert.Equal(Specialist.GeneralPractice, spec);
    }

    [Fact]
    public async Task DecideAsync_DefaultsToGeneralPractice_OnModerateLevelAndNeutralSymptoms()
    {
        var agent = new SpecialistAgent();
        var spec = await agent.DecideAsync("dor leve nas costas", TriageLevel.Moderate);
        Assert.Equal(Specialist.GeneralPractice, spec);
    }

    [Fact]
    public async Task DecideAsync_DefaultsToGeneralPractice_OnHighLevelAndNeutralSymptoms()
    {
        var agent = new SpecialistAgent();
        var spec = await agent.DecideAsync("mal estar inespecífico", TriageLevel.High);
        Assert.Equal(Specialist.GeneralPractice, spec);
    }

    [Fact]
    public async Task DecideAsync_ReturnsEmergency_OnCriticalLevelAndNeutralSymptoms()
    {
        var agent = new SpecialistAgent();
        var spec = await agent.DecideAsync("mal estar intenso", TriageLevel.Critical);
        Assert.Equal(Specialist.Emergency, spec);
    }

    [Fact]
    public async Task DecideAsync_PrioritizesSymptomRules_OverCriticalFallback()
    {
        var agent = new SpecialistAgent();
        var spec = await agent.DecideAsync("dor no peito", TriageLevel.Critical);
        Assert.Equal(Specialist.Cardiology, spec);
    }
}
