using HealthTriageAI.ApiService.Services;

namespace HealthTriageAI.Tests;

public class RiskAgentTests
{
    [Fact]
    public async Task EvaluateAsync_ComputesLow_WithMildFever()
    {
        var agent = new RiskAgent();
        var metrics = new Dictionary<string, double> { ["temperature"] = 38.9 };
        var level = await agent.EvaluateAsync(metrics, 25);
        Assert.Equal(ApiService.Enums.TriageLevel.Low, level);
    }

    [Fact]
    public async Task EvaluateAsync_ComputesModerate_WhenLowSystolicAndNotEnoughOlder()
    {
        var agent = new RiskAgent();
        var metrics = new Dictionary<string, double> { ["systolic"] = 85 };
        var level = await agent.EvaluateAsync(metrics, 65);
        Assert.Equal(ApiService.Enums.TriageLevel.Moderate, level);
    }

    [Fact]
    public async Task EvaluateAsync_ComputesHigh_WhenLowSyncopeAndMildFeverAndOlder()
    {
        var agent = new RiskAgent();
        var metrics = new Dictionary<string, double> { ["flag_syncope"] = 1, ["temperature"] = 39.5 };
        var level = await agent.EvaluateAsync(metrics, 85);
        Assert.Equal(ApiService.Enums.TriageLevel.High, level);
    }

    [Fact]
    public async Task EvaluateAsync_ComputesCritical_WhenLowSystolicAndChestPain()
    {
        var agent = new RiskAgent();
        var metrics = new Dictionary<string, double> { ["systolic"] = 85, ["flag_chest_pain"] = 1 };
        var level = await agent.EvaluateAsync(metrics, 45);
        Assert.Equal(ApiService.Enums.TriageLevel.Critical, level);
    }
}
