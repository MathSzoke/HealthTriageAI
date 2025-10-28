using HealthTriageAI.ApiService.Enums;
using HealthTriageAI.ApiService.Services.Abstractions;
using Microsoft.Agents.AI;
using OpenAI;

namespace HealthTriageAI.ApiService.Services;

public class FirstAidAgent : IFirstAidAgent
{
    private readonly AIAgent _agent;

    public FirstAidAgent(IConfiguration config)
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_KEY") ?? config["OpenAI:ApiKey"];
        var model = Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? config["OpenAI:Model"];
        var client = new OpenAIClient(apiKey);
        var chat = client.GetChatClient(model);
        this._agent = chat.CreateAIAgent(
            instructions: """
                Gere instruções de primeiros cuidados breves, seguras e passo-a-passo para o paciente aguardar atendimento.
                Nunca faça diagnóstico, nem recomende medicamentos controlados.
                Gere instruções objetivas em um paragrafo pequeno.
            """,
            name: "first_aid");
    }

    public async Task<string> GenerateAsync(string symptoms, double? temperature, int? heartRate, int? systolicBP, TriageLevel level)
    {
        var prompt = $"""
            Sintomas: {symptoms}.
            Temp: {temperature?.ToString() ?? "N/A"} °C,
            FC: {heartRate?.ToString() ?? "N/A"} bpm,
            PAS: {systolicBP?.ToString() ?? "N/A"} mmHg.
            Gravidade: {level}.
            Gere passos de primeiros cuidados para aguardar atendimento.
        """;
            
        var res = await this._agent.RunAsync(prompt);
        return res.ToString();
    }
}