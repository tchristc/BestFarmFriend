# BigBestFarm — Development Plan
Version 1.0

---

## Stack Summary

| Item | Choice | Reason |
|---|---|---|
| Framework | Blazor Web App (.NET 10) | Full-stack C#; SSR + InteractiveAuto for best perf |
| Weather API | Open-Meteo | Free, no key, comprehensive, supports US units |
| Geocoding | Open-Meteo Geocoding | Same domain, free, city + ZIP search |
| AI | OpenAI API (GPT-4o) / GitHub Models | Configurable; GPT-4o is best for ag advisory text |
| Database | SQLite -> PostgreSQL | SQLite zero-config for dev; swap for prod |
| ORM | EF Core 10 | Standard .NET, good migration tooling |
| CSS | Bootstrap 5 | Rapid responsive layout; well known |
| Auth | None (Phase 1); ASP.NET Identity (Phase 3) | Keep MVP simple |

---

## Phase 1 — MVP Sprint Plan

### Sprint 1: Solution Scaffolding (Days 1-2)
Tasks:
1. Create solution: dotnet new blazorwasm --hosted -n BigBestFarm OR dotnet new blazorwebapp
   - Use "Blazor Web App" template (.NET 10) with InteractiveAuto render mode
   - Projects: BigBestFarm.Web, BigBestFarm.Core, BigBestFarm.Infrastructure
2. Add project references: Web -> Core, Web -> Infrastructure, Infrastructure -> Core
3. Add NuGet packages:
   - Microsoft.EntityFrameworkCore.Sqlite
   - Microsoft.EntityFrameworkCore.Design
   - Serilog.AspNetCore
   - OpenAI (OpenAI .NET SDK, official package)
4. Set up solution folder structure per spec
5. Configure Serilog in Program.cs
6. Add appsettings.json with placeholder sections: Weather, AI, ConnectionStrings

### Sprint 2: Core Domain Models (Days 2-3)
Files to create in BigBestFarm.Core/Models/:
- Location.cs
- WeatherSnapshot.cs
- WeatherForecastDay.cs
- WeatherForecastHour.cs
- Crop.cs
- CropCategory.cs (enum)
- CropAction.cs
- ActionType.cs (enum)
- SprayFactor.cs (enum: Wind, Precipitation, Temperature, Humidity, TimeOfDay)
- FactorStatus.cs (enum: Pass, Marginal, Fail)
- SprayBand.cs (enum: Go, Caution, Marginal, NoGo)
- SprayFactorResult.cs
- SprayReadinessResult.cs
- ProductType.cs (enum: Copper, Sulfur, Oil, Insecticide, Herbicide)
- ProductReadinessResult.cs
- GrowthStage.cs (enum + display)
- CropAdvisory.cs
- AdvisoryStatus.cs (enum)
- UserSettings.cs

### Sprint 3: Infrastructure — Weather & Geocoding (Days 3-4)
Files in BigBestFarm.Infrastructure/WeatherApi/:
- OpenMeteoClient.cs        -- HttpClient wrapper for current + forecast
- OpenMeteoCurrentDto.cs    -- JSON deserialization DTOs
- OpenMeteoForecastDto.cs
- WeatherMapper.cs          -- DTOs -> domain models

Files in BigBestFarm.Infrastructure/GeocodingApi/:
- GeocodingClient.cs        -- Open-Meteo geocoding search
- GeocodingResultDto.cs
- GeocodingMapper.cs

Register HttpClient factories in DI:
- Named client "OpenMeteo" with base address
- Named client "Geocoding" with base address

### Sprint 4: Core Services — Spray Rule Engine (Days 4-5)
Files in BigBestFarm.Core/Rules/:
- WindRule.cs
- PrecipitationRule.cs
- TemperatureRule.cs
- HumidityRule.cs
- TimeOfDayRule.cs
- CopperProductRule.cs
- SulfurProductRule.cs
- OilProductRule.cs
- InsecticideRule.cs
- HerbicideRule.cs

Files in BigBestFarm.Core/Services/:
- SprayRuleEngine.cs          -- Aggregate all rules -> SprayReadinessResult
- WeatherService.cs           -- Wraps OpenMeteoClient, adds caching (IMemoryCache)
- GeocodingService.cs         -- Wraps GeocodingClient
- LocationService.cs          -- Manages saved locations; uses ILocationRepository
- CropAdvisoryService.cs      -- Per-crop action evaluation
- GrowthStageService.cs       -- Date + crop -> GrowthStage estimate
- AiAdvisoryService.cs        -- Prompt builder + OpenAI call
- CalendarService.cs          -- Seasonal stage + action calendar builder

### Sprint 5: Data Layer (Days 5-6)
Files in BigBestFarm.Infrastructure/Data/:
- AppDbContext.cs              -- EF Core DbContext; DbSets for Crop, CropAction, Location, etc.
- Migrations/                 -- EF migrations
- Seed/
  - CropSeeder.cs             -- All crops from spec
  - CropActionSeeder.cs       -- Default actions per crop
  - GrowthStageSeeder.cs      -- Growth stage calendar data per crop

Files in BigBestFarm.Infrastructure/Repositories/:
- LocationRepository.cs
- CropRepository.cs
- CropAdvisoryRepository.cs

### Sprint 6: Blazor UI — Layout & Shared Components (Days 6-8)
Files in BigBestFarm.Web/Components/:

Layout/:
- MainLayout.razor            -- Sidebar nav + top bar + content
- NavMenu.razor               -- Links: Dashboard, Crops, Calendar, Settings
- TopBar.razor                -- Location quick-select, dark mode toggle

Shared/WeatherSvg/:
- SunnyIcon.razor
- PartlyCloudyIcon.razor
- CloudyIcon.razor
- RainIcon.razor
- ThunderstormIcon.razor
- SnowIcon.razor
- FogIcon.razor
- WindRose.razor              -- SVG direction indicator
- PressureGauge.razor         -- SVG trend arrow
- PrecipChart.razor           -- SVG hourly bar chart
- TemperatureGauge.razor

Shared/:
- LocationPicker.razor        -- Autocomplete city/zip input + saved list dropdown
- WeatherCard.razor           -- Current conditions summary card
- HourlyForecastStrip.razor   -- 24h scrollable strip
- DailyForecastGrid.razor     -- 7-day cards
- SprayScoreBadge.razor       -- Big score circle with color band
- SprayFactorRow.razor        -- Single factor: icon + value + status badge
- CropCard.razor              -- Quick crop summary card for dashboard
- ActionStatusBadge.razor     -- Recommended / Caution / NoGo pill
- AiAdvisoryCard.razor        -- AI response display card with loading state

### Sprint 7: Pages (Days 8-10)
Pages/:
- Dashboard/Dashboard.razor   -- Weather hero + spray score + crop card grid
- Crops/CropList.razor        -- All crops with filter; activate/deactivate
- Crops/CropDetail.razor      -- Full action advisory for one crop + AI button
- Spray/SprayDetail.razor     -- Full spray readiness breakdown
- Calendar/CropCalendar.razor -- Month grid + checklist
- Settings/Settings.razor     -- Location mgmt, AI key, unit toggle, crop config

### Sprint 8: Wiring & Polish (Days 10-12)
- Cascading AppState (location, unit preference, active crops)
- JavaScript interop for browser geolocation
- JavaScript interop for localStorage (settings + saved locations)
- Dark mode CSS variables
- Loading skeleton components
- Error boundary components
- appsettings config for AI key (dev secrets / env var in prod)
- Final responsive layout pass

### Sprint 9: Test & Build Validation (Days 12-14)
- Unit tests for SprayRuleEngine (all boundary conditions from orchard-spec)
- Unit tests for GrowthStageService
- Unit tests for WeatherMapper
- Integration test for OpenMeteoClient against live API (or recorded responses)
- Smoke test each page renders without errors
- Run dotnet build + dotnet test

---

## Key Implementation Notes

### Open-Meteo API Call — Current Weather
GET https://api.open-meteo.com/v1/forecast
  ?latitude={lat}
  &longitude={lon}
  &current=temperature_2m,relative_humidity_2m,apparent_temperature,precipitation,
            weather_code,wind_speed_10m,wind_direction_10m,wind_gusts_10m,
            surface_pressure,visibility,uv_index,dew_point_2m
  &hourly=temperature_2m,precipitation_probability,precipitation,wind_speed_10m,wind_gusts_10m
  &daily=sunrise,sunset,temperature_2m_max,temperature_2m_min,precipitation_sum
  &wind_speed_unit=mph
  &temperature_unit=fahrenheit
  &precipitation_unit=inch
  &forecast_days=7
  &timezone=auto

### Spray Score Algorithm
score = 0
score += WindScore(windSpeed, gustSpeed) * 0.30         // 0-100
score += PrecipScore(past2h, forecast1h, forecast4h) * 0.25
score += TempScore(tempF) * 0.20
score += HumidityScore(rh, dewPointGap) * 0.15
score += TimeOfDayScore(now, sunrise, sunset) * 0.10

Band:
  score >= 80 -> Go (green)
  score >= 60 -> Caution (yellow)
  score >= 40 -> Marginal (orange)
  score < 40  -> NoGo (red)

### AI Prompt Template
System: "You are an expert agricultural advisor specializing in {CropName} farming.
         Provide concise, practical, actionable advice."

User:   "Today is {Date}. Location: {LocationName}.
         Current weather: Temp {TempF}F, Feels like {FeelsLikeF}F,
         Wind {WindMph} mph {WindDir} gusting to {GustMph} mph,
         Humidity {RH}%, Dew Point {DewF}F,
         Precipitation past 24h: {Precip24h} in,
         Forecast next 4h: {ForecastPrecip} in,
         Pressure: {Pressure} hPa ({PressureTrend}).
         Crop: {CropName}. Estimated growth stage: {GrowthStage}.
         Spray Readiness Score: {Score}/100 ({Band}).
         Failing conditions: {FailingConditions}.
         Please provide: (1) overall recommendation for today, (2) specific spray advice,
         (3) any other important farming actions for this crop today,
         (4) what to watch for in the next 24-48 hours."

### Caching Strategy
- Weather data: IMemoryCache, 15-minute TTL per location
- Geocoding results: IMemoryCache, 24-hour TTL
- AI responses: IMemoryCache, keyed by crop+location+date, TTL until midnight local
- EF Core query caching: disabled for advisory queries (real-time)

### localStorage Keys
- bbf_settings           -- UserSettings JSON
- bbf_locations          -- SavedLocation[] JSON
- bbf_last_location      -- last used LocationId
- bbf_dark_mode          -- "dark" | "light"
- bbf_checklists         -- { cropId_year_week: bool[] }

---

## NuGet Package List

BigBestFarm.Web:
- Microsoft.EntityFrameworkCore.Sqlite
- Microsoft.EntityFrameworkCore.Design
- Serilog.AspNetCore
- Serilog.Sinks.File
- OpenAI (v2.x official)

BigBestFarm.Infrastructure:
- Microsoft.EntityFrameworkCore.Sqlite
- Microsoft.Extensions.Http

BigBestFarm.Core:
- Microsoft.Extensions.Caching.Abstractions
- Microsoft.Extensions.DependencyInjection.Abstractions

BigBestFarm.Tests.Unit:
- xunit
- Moq (or NSubstitute)
- FluentAssertions

---

## File Count Estimate — Phase 1

| Area | Files |
|---|---|
| Domain models | ~20 |
| Rule classes | ~10 |
| Services | ~8 |
| Infrastructure (API + Data) | ~15 |
| Blazor components | ~25 |
| Pages | ~7 |
| Tests | ~15 |
| Config + misc | ~5 |
| **Total** | **~105** |
