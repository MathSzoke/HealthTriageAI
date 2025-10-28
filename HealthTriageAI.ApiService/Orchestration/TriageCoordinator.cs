using HealthTriageAI.ApiService.Hubs;
using HealthTriageAI.ApiService.Models;
using HealthTriageAI.ApiService.Services;
using HealthTriageAI.ApiService.Services.Abstractions;
using Microsoft.AspNetCore.SignalR;

namespace HealthTriageAI.ApiService.Orchestration;

public class TriageCoordinator(SymptomAgent symptom, RiskAgent risk, SpecialistAgent specialist, IAdviceAgent advice, IFirstAidAgent firstAid, IHubContext<TriageHub> hub)
{
    private readonly SpecialistAgent _specialist = specialist;
    private readonly IAdviceAgent _advice = advice;
    private readonly IFirstAidAgent _firstAid = firstAid;
    private readonly Dictionary<string, TriageCase> _cases = [];

    public async Task<string> StartAsync(TriageInput input)
    {
        var id = Guid.NewGuid().ToString("N");
        var c = new TriageCase(id, input.Name, input.Age, input.Symptoms, input.Temperature, input.HeartRate, input.SystolicBP, input.Location, null, null, string.Empty, string.Empty, "received", DateTimeOffset.UtcNow);
        this._cases[id] = c;
        await hub.Clients.All.SendAsync("case_updated", c);

        var metrics = await symptom.AnalyzeAsync(input.Symptoms, input.Temperature, input.HeartRate, input.SystolicBP);
        c = c with { Status = "symptom_analyzed" };
        this._cases[id] = c;
        await hub.Clients.All.SendAsync("case_step", new { id, step = "symptom", payload = metrics });
        await hub.Clients.All.SendAsync("case_updated", c);

        var level = await risk.EvaluateAsync(metrics, input.Age);
        c = c with { Level = level, Status = "risk_evaluated" };
        this._cases[id] = c;
        await hub.Clients.All.SendAsync("case_step", new { id, step = "risk", payload = level.ToString() });
        await hub.Clients.All.SendAsync("case_updated", c);

        var specialist = await this._specialist.DecideAsync(input.Symptoms, level);
        c = c with { Specialist = specialist, Status = "specialist_selected" };
        this._cases[id] = c;
        await hub.Clients.All.SendAsync("case_step", new { id, step = "specialist", payload = specialist.ToString() });
        await hub.Clients.All.SendAsync("case_updated", c);

        var firstAid = await this._firstAid.GenerateAsync(input.Symptoms, input.Temperature, input.HeartRate, input.SystolicBP, level);
        c = c with { FirstAid = firstAid, Status = "first_aid_ready" };
        this._cases[id] = c;
        await hub.Clients.All.SendAsync("case_step", new { id, step = "first_aid", payload = firstAid });
        await hub.Clients.All.SendAsync("case_updated", c);

        var advice = await this._advice.GenerateAsync(level, specialist);
        c = c with { Advice = advice, Status = "advice_ready" };
        this._cases[id] = c;
        await hub.Clients.All.SendAsync("case_step", new { id, step = "advice", payload = advice });
        await hub.Clients.All.SendAsync("case_updated", c);

        return id;
    }
}