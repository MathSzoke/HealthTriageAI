# 🩺 HealthTriage AI — Intelligent Medical Triage

<p align="center">
  <img src="https://healthtriage.mathszoke.com/banner.png" alt="HealthTriageAI Banner" width="800"/>
</p>

<p align="center">
  <b>An intelligent triage system built with .NET and the Microsoft Agent Framework, simulating real-time medical evaluations powered by AI agents that assess risk, recommend specialists, and provide first-aid guidance.</b>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white"/>
  <img src="https://img.shields.io/badge/Microsoft%20Agent%20Framework-0078D4?style=for-the-badge&logo=microsoft&logoColor=white"/>
  <img src="https://img.shields.io/badge/SignalR-CC0000?style=for-the-badge&logo=signalr&logoColor=white"/>
  <img src="https://img.shields.io/badge/React-61DAFB?style=for-the-badge&logo=react&logoColor=black"/>
  <img src="https://img.shields.io/badge/Fluent%20UI-0078D4?style=for-the-badge&logo=microsoft&logoColor=white"/>
</p>

---

## 🧠 About the Project

**HealthTriage AI** is a simulated healthcare triage platform where multiple **AI agents** collaborate in real time to evaluate a patient’s condition.

The goal is to demonstrate how **agent orchestration** can solve real-world healthcare scenarios without handling sensitive medical data.

Each triage request goes through a complete evaluation pipeline:
- **Risk analysis** — determining urgency based on symptoms and vitals.
- **Specialist recommendation** — matching symptoms to the correct medical specialty.
- **AI advice** — generating a short and safe orientation message.
- **First-aid guide** — providing immediate, non-diagnostic care instructions.

All this happens live in the UI through a **SignalR hub**, showing how the case evolves step by step — as if multiple digital assistants were collaborating like a real medical team.

---

## ⚙️ Core Stack

| Layer | Technologies |
|:--|:--|
| **Backend** | .NET 10 + Microsoft Agent Framework + SignalR |
| **Frontend** | React + Fluent UI + Vite |
| **AI Connectors** | OpenAI (GPT) |
| **Testing** | xUnit + custom hub capture for SignalR calls |
| **Architecture** | Clean Architecture with service orchestration pattern |

---

## 🧩 Project Structure

```
src/
 ├─ HealthTriageAI.ApiService/      → Core API + Agent Orchestration
 │                                   - Hosts SignalR hub (/hubs/triage)
 │                                   - Coordinates 5 agents (Symptom, Risk, Specialist, Advice, FirstAid)
 │                                   - Exposes endpoints for case reporting (/api/triage/report)
 │                                   - Uses Microsoft Agent Framework for LLM orchestration
 │
 ├─ HealthTriageAI.Web/             → Frontend (React + Fluent UI)
 │                                   - Real-time dashboard for triage cases
 │                                   - Displays live case flow (symptom → risk → specialist → advice)
 │                                   - Responsive layout with filtering and search
 │                                   - Connects directly to SignalR hub
 │
 └─ HealthTriageAI.Tests/           → Automated Tests (xUnit)
                                     - Unit tests for RiskAgent, SpecialistAgent, TriageCoordinator
                                     - Custom fake hub context for SignalR message capture
```

---

## 🌟 Key Features

- 🔄 **Real-time triage pipeline** via SignalR  
- 🧩 **Agent orchestration** using Microsoft Agent Framework  
- 💬 **GPT integration** for dynamic and contextual medical advice  
- 🧪 **Extensive testing** without external API costs (mocked advice agents)  
- 🩹 **First-aid assistant** with contextual pre-care messages  
- 💻 **Modern Fluent UI dashboard** with responsive layout and filtering  

---

## 💡 How It Works

1. The user submits a **triage form** (name, age, symptoms, vitals).  
2. The backend triggers a **TriageCoordinator** that orchestrates five agents:
   - **SymptomAgent** → extracts key data from text.  
   - **RiskAgent** → classifies the urgency level.  
   - **SpecialistAgent** → selects the correct specialty.  
   - **AdviceAgent** → generates safe instructions.  
   - **FirstAidAgent** → produces immediate non-diagnostic care tips.  
3. Each step emits real-time updates to the UI via **SignalR**.  
4. The frontend displays the evolution of each case visually — from report to advice.  

---

## 🧪 Testing Strategy

Unit and integration tests ensure system stability without incurring AI usage costs.

| Test Area | Description |
|:--|:--|
| **RiskAgentTests** | Evaluates risk classification logic based on vitals. |
| **SpecialistAgentTests** | Maps symptom keywords to medical specialties. |
| **TriageCoordinatorTests** | Validates SignalR sequence and orchestration flow using a fake hub context. |

> AI agents are mocked in tests via `FakeAdviceAgent` and `FakeFirstAidAgent` to prevent token usage.

---

## ⚠️ Guarantees

Before starting, you have to create a new file in HealthTriageAI.ApiService project named "appsettings.json", and put this value inside:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "OpenAI": {
    "ApiKey": "<ApiKey>",
    "Model": "gpt-4o-mini"
  }
}
```

Please, make sure you go change "<ApiKey>" for your real api key from OpenAI, follow site to get it:
[OpenAI Api keys](https://platform.openai.com/api-keys)

---

## 🚀 How to Run

### Backend
```bash
cd HealthTriageAI.ApiService
dotnet run
```

### Frontend
```bash
cd HealthTriageAI.Web
npm install
npm run dev
```

### Tests
```bash
cd HealthTriageAI.Tests
dotnet test
```

---

## 📫 Contact

📧 **Email:** [matheusszoke@gmail.com](mailto:matheusszoke@gmail.com)  
💼 **LinkedIn:** [linkedin.com/in/matheusszoke](https://linkedin.com/in/matheusszoke)

---

<p align="center">
  <sub>Made with 💚 by <strong>Matheus Szoke</strong> — powered by the Microsoft Agent Framework</sub>
</p>
