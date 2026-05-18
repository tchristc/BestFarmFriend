# 🍎 BestFarmFriend

> A full-stack **Blazor** web application that gives farmers real-time weather intelligence, AI-powered crop advice, and spray-readiness scoring — all tailored to their specific farm location.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Blazor](https://img.shields.io/badge/Blazor-WebAssembly%20%2B%20Server-512BD4?logo=blazor)](https://blazor.net)
[![Azure](https://img.shields.io/badge/Deployed-Azure%20App%20Service-0078D4?logo=microsoftazure)](https://azure.microsoft.com/)

---

## 🚀 Live Demo

**[https://www.bestfarmfriend.com/](https://www.bestfarmfriend.com/)**

---

## 📸 Screenshots
### Dashboard
<img width="1249" height="1245" alt="Real-time weather + AI advisory" src="https://github.com/user-attachments/assets/446bd74e-9037-4eae-bc55-f2fb6eee5c92" />

### Crop Calendar
<img width="1276" height="1256" alt="Location-aware crop growth stages" src="https://github.com/user-attachments/assets/bb9959e7-13bd-483a-8298-b6e9ba9d33fe" />

### Spray Detail
<img width="1276" height="1256" alt="Factor-by-factor spray readiness breakdown" src="https://github.com/user-attachments/assets/e5b24ff9-3d43-48ee-bf9b-bdedb9a25157" />

---

## ✨ Features

- **📍 Location-Aware Dashboard** — Search and save farm locations; all times, weather, and advice adapt to the selected location's timezone automatically (cross-platform IANA timezone support via TimeZoneConverter)
- **🌤️ Live Weather** — Current conditions and multi-day forecast fetched from the [Open-Meteo API](https://open-meteo.com/) (free, no key required)
- **🧪 Spray Readiness Engine** — Rule-based scoring system evaluating wind speed, precipitation, temperature, humidity, and time-of-day relative to local sunrise/sunset
- **🤖 AI Crop Advisory** — GPT-4o powered narrative advice tailored to the crop, growth stage, and current weather conditions (user provides their own OpenAI API key)
- **📅 Crop Calendar** — Visual monthly calendar with growth stage tracking and "today" highlighting based on location-local date
- **🌱 Crop Library** — Detailed profiles for common crops including growth stages, key notes, and recommended actions
- **⚙️ Settings** — Manage saved locations, configure OpenAI API key, and customize app preferences

---

## 🏗️ Architecture

```
BestFarmFriend/
├── src/
│   ├── BestFarmFriend.Core/            # Domain models, interfaces, business rules
│   │   ├── Models/                     # Location, Crop, WeatherSnapshot, SprayReadinessResult, ...
│   │   ├── Interfaces/                 # IWeatherService, ICropRepository, IAiAdvisoryService, ...
│   │   └── Rules/                      # WindRule, HumidityRule, TemperatureRule, TimeOfDayRule, ...
│   │
│   ├── BestFarmFriend.Infrastructure/  # External integrations and data access
│   │   ├── WeatherApi/                 # Open-Meteo HTTP client + response mapping
│   │   ├── GeocodingApi/               # Location search via Open-Meteo Geocoding API
│   │   ├── AiApi/                      # OpenAI GPT-4o advisory service
│   │   ├── Repositories/               # EF Core repositories (Location, Crop)
│   │   ├── Data/                       # AppDbContext + EF migrations + seeding
│   │   └── Services/                   # SprayRuleEngine, GrowthStageService
│   │
│   ├── BestFarmFriend.Web/             # Blazor Server host (SSR + Interactive Server)
│   │   ├── Components/Pages/           # Dashboard, CropList, CropDetail, CropCalendar, Settings
│   │   ├── Components/Shared/          # LocationPicker, NavMenu, layout components
│   │   └── Services/                   # AppState (shared UI state)
│   │
│   └── BestFarmFriend.Web.Client/      # Blazor WebAssembly client project
│
└── tests/
    └── BestFarmFriend.Tests.Unit/      # xUnit unit tests for rules and services
```

**Key design decisions:**
- Clean Architecture with dependency inversion — Core has zero infrastructure dependencies
- Rule-based spray engine using the Strategy pattern — each factor (wind, precip, temp, etc.) is an independent, testable class
- `EnsureCreated()` → EF Core Migrations path with a backwards-compatible startup bootstrap for existing databases

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|-----------|
| Framework | .NET 10, Blazor Web App (Server + WASM hybrid) |
| UI | Bootstrap 5, custom scoped CSS |
| Database | SQLite + Entity Framework Core 10 (Code-First, Migrations) |
| Weather Data | Open-Meteo API (free, no API key required) |
| AI | OpenAI SDK (`gpt-4o`) |
| Timezone | TimeZoneConverter (IANA ↔ Windows, cross-platform) |
| Logging | Serilog (Console + rolling file) |
| CI/CD | GitHub Actions → Azure App Service |
| Testing | xUnit |

---

## ⚡ Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- (Optional) OpenAI API key for AI advisory features

### Run Locally

```bash
git clone https://github.com/tchristc/BestFarmFriend.git
cd BestFarmFriend
dotnet run --project src/BestFarmFriend.Web/BestFarmFriend.Web
```

Navigate to `https://localhost:5001`. The SQLite database is created automatically on first run.

### Configuration

| Setting | Where |
|---------|-------|
| OpenAI API Key | App Settings page (stored in SQLite, not config files) |
| Database path | `appsettings.json` → `ConnectionStrings:DefaultConnection` |

---

## 🧪 Running Tests

```bash
dotnet test tests/BestFarmFriend.Tests.Unit
```

---

## 🚀 Deployment

The app deploys automatically to **Azure App Service** via GitHub Actions on every push to `main`.

**Workflow:** `.github/workflows/deploy.yml`
- Restores dependencies
- Publishes the Blazor hosted project (WASM assets bundled)
- Deploys to Azure using a publish profile secret

The app uses EF Core `MigrateAsync()` at startup, so schema changes (including new columns) are applied automatically to the live SQLite database without manual intervention.

---

## 🗺️ Roadmap

- [ ] Push notifications for ideal spray windows
- [ ] Historical weather trend charts
- [ ] Multi-user / farm team support
- [ ] Mobile PWA support

---

## 👤 Author

**Tom Christensen**
- GitHub: [@tchristc](https://github.com/tchristc)

---
