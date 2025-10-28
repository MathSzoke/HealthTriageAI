using HealthTriageAI.ApiService.Enums;
using HealthTriageAI.ApiService.Services.Abstractions;
using Microsoft.Agents.AI;
using OpenAI;

namespace HealthTriageAI.ApiService.Services;

public class AdviceAgent : IAdviceAgent
{
    private readonly AIAgent _agent;

    public AdviceAgent(IConfiguration config)
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_KEY") ?? config["OpenAI:ApiKey"];
        var model = Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? config["OpenAI:Model"];
        var client = new OpenAIClient(apiKey);
        var chat = client.GetChatClient(model);
        this._agent = chat.CreateAIAgent(
            instructions: """
                Você é um agente de triagem.
                Gere orientações curtas, claras e seguras, sem diagnóstico definitivo,
                sempre sugerindo procurar um profissional de saúde quando necessário.
            """,
            name: "advice");
    }

    public async Task<string> GenerateAsync(TriageLevel level, Specialist specialist)
    {
        var prompt = $"""
            Nível: {level}.
            Especialidade sugerida: {specialist}. 
            Gere orientações objetivas em 2 a 3 frases para o paciente.
        """;

        var res = await this._agent.RunAsync(prompt);
        return res.ToString();
    }
}