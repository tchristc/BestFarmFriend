# BigBestFarm — Application Specification
Version 1.0 | Blazor .NET 10 | Web Application

---

## 1. Purpose & Vision

BigBestFarm is a weather-driven farm advisory web application built with Blazor (.NET 10).
Its primary goal is to help farmers make informed decisions about daily field operations —
especially spraying — by combining real-time weather data, crop-specific rule engines, and
AI-generated narrative recommendations.

The application was inspired by orchard spray decision-making rules (see orchard-spray-spec.md)
and is designed to expand to all major crop types.

---

## 2. Target Users

- Orchard operators (primary)
- Market gardeners and truck farmers
- Vineyard managers
- Berry and small fruit growers
- Row crop farmers (corn, soy, wheat, vegetable)

---

## 3. Core Features

### 3.1 Location Management
- Auto-detect user location via browser Geolocation API
- Manual location entry via ZIP code or city name with dynamic autocomplete
  - Backed by Open-Meteo Geocoding API (free, no key)
- Save and name favorite/previous locations (stored in browser localStorage + optional server profile)
- Quick-select saved locations from a dropdown or sidebar list
- Each location stores: display name, latitude, longitude, timezone

### 3.2 Weather Dashboard
Displayed prominently for the selected location:

| Metric | Details |
|---|---|
| Date & Time | Current local time for location |
| Temperature | Current, feels-like, daily high/low |
| Conditions | Sky description (sunny, overcast, etc.) |
| Precipitation | Current rate, past 1h, past 24h, forecast next 4h |
| Wind | Speed (mph), direction, gust speed |
| Humidity | Relative humidity % |
| Dew Point | Calculated or returned from API |
| Barometric Pressure | Current + trend (rising/falling/steady) |
| UV Index | UV risk level |
| Visibility | Miles |
| Sunrise / Sunset | Local times |
| Hourly Forecast | 24-hour strip with temp, precip probability, wind |
| 7-Day Forecast | Daily summary cards |

Weather data sourced from **Open-Meteo** (https://api.open-meteo.com) — free, no API key required.

### 3.3 Weather SVG Graphics
- Animated SVG weather icons for: sunny, partly cloudy, overcast, rain, drizzle, thunderstorm, snow, fog, wind
- SVG wind rose / direction indicator
- SVG pressure trend arrow
- SVG precipitation bar chart (hourly)
- Color-coded temperature gauge

### 3.4 Spray Readiness Dashboard
- Prominently shows overall Spray Readiness Score (0-100) with color band (green/yellow/orange/red)
- Breakdown widget per factor: Wind, Precipitation, Temperature, Humidity, Time of Day
- Each factor shows its current value and a pass/marginal/fail badge
- One-line reason for any failing or marginal condition
- Product-level readiness for: Copper, Sulfur, Oil, Insecticide, Herbicide

### 3.5 Crop Advisory System
Users configure which crops they grow. For each crop, the advisory system provides:

#### 3.5.1 Supported Crops (initial set)
Orchards & Tree Fruit:
- Apple, Pear, Cherry (Sweet), Cherry (Sour), Peach, Nectarine, Plum, Prune, Apricot, Quince, Fig
- Walnut, Almond, Hazelnut, Pecan

Vines & Berries:
- Grape (Wine), Grape (Table), Blueberry, Raspberry, Blackberry, Strawberry, Cranberry

Vegetables:
- Tomato, Potato, Pepper, Cucumber, Squash, Pumpkin, Corn, Bean (snap/dry), Lettuce/Greens, Brassicas (cabbage family)

Row Crops:
- Wheat, Soybean, Corn (field), Oats, Sorghum, Alfalfa

#### 3.5.2 Crop Actions
For each crop, the user can evaluate:
- **Spraying** — insecticide, fungicide, herbicide (primary feature, orchard-spec rules apply)
- **Fertilizing** — soil vs. foliar; weather suitability
- **Tilling** — soil moisture / compaction window
- **Grooming** — pruning, training, thinning; temperature and growth stage gates
- **Cleaning** — equipment wash-down, post-harvest cleanup
- **Irrigation** — soil moisture deficit, ET-based trigger
- **Harvesting** — temperature, rain, and wind thresholds for harvest readiness

#### 3.5.3 Action Suggestions
Each action has configurable suggestion rules:
- Rule conditions stored per crop-action pair
- Rules reference WeatherMetrics (temperature, wind, humidity, precipitation, etc.)
- Rules evaluate to: Recommended / Acceptable / Caution / Not Recommended
- Users can override default rules with custom thresholds in Settings
- Suggestions are displayed as a card per action with icon, status badge, and short reasoning text

### 3.6 Crop Calendar
Each crop has a seasonal calendar view:
- Month/week grid with phenological growth stages
- Suggested actions for each stage (color-coded by action type)
- Checklist of tasks with user-checkable completion tracking
- Today marker overlaid on calendar
- Printable view

### 3.7 AI Advisory
For each crop + current weather + growth stage, users can request an AI narrative:
- Button: "Ask AI for today's recommendation"
- Prompt is auto-constructed from: crop name, growth stage, all current weather metrics, any failing spray conditions
- Response displayed as a formatted advisory card
- AI provider configurable in Settings: OpenAI (GPT-4o), Azure OpenAI, GitHub Models (Copilot)
- API key stored in user settings (encrypted in localStorage or server-side secret)
- Response is cached per crop/location/date to avoid redundant calls

---

## 4. Technical Architecture

### 4.1 Technology Stack
| Layer | Technology |
|---|---|
| Frontend | Blazor Web App (.NET 10, InteractiveAuto render mode) |
| Backend | ASP.NET Core (.NET 10) minimal API + Blazor SSR |
| ORM | Entity Framework Core 10 |
| Database | SQLite (dev) / PostgreSQL or SQL Server (prod) |
| CSS | Bootstrap 5 + custom CSS variables |
| Icons | Custom SVG weather icons + Bootstrap Icons |
| AI | OpenAI .NET SDK (or HttpClient to GitHub Models) |
| Weather | Open-Meteo HttpClient service |
| Geocoding | Open-Meteo Geocoding API |
| Auth (optional) | ASP.NET Core Identity (phase 2) |
| State | Blazor cascading state + localStorage interop |
| Logging | Serilog to file + console |

### 4.2 Project Structure
```
BigBestFarm/
  BigBestFarm.sln
  src/
    BigBestFarm.Web/           -- Blazor Web App (host + WASM client)
      Components/
        Layout/
        Shared/
          WeatherSvg/
          LocationPicker/
          CropCard/
          SprayReadiness/
          AiAdvisoryCard/
        Pages/
          Dashboard/
          Crops/
          Calendar/
          Settings/
      wwwroot/
        css/
        svg/
    BigBestFarm.Core/          -- Domain models, interfaces, rule engine
      Models/
      Interfaces/
      Rules/
      Services/                -- domain logic only
    BigBestFarm.Infrastructure/ -- EF Core, weather API client, AI client
      Data/
        Migrations/
        Seed/
      WeatherApi/
      GeocodingApi/
      AiApi/
      Repositories/
  specs/
  plan/
  tests/
    BigBestFarm.Tests.Unit/
    BigBestFarm.Tests.Integration/
```

### 4.3 Key Domain Models

#### Location
```
Id, DisplayName, Latitude, Longitude, Timezone, ZipCode, IsFavorite, LastUsed
```

#### WeatherSnapshot
```
LocationId, ObservedAt, TemperatureF, FeelsLikeF, DewPointF, HumidityPct,
WindSpeedMph, WindGustMph, WindDirectionDeg, PressureHpa, PressureTrend,
PrecipitationRateIn, PrecipPast1hIn, PrecipPast24hIn, UvIndex,
VisibilityMiles, Condition (enum), SunriseUtc, SunsetUtc
```

#### Crop
```
Id, Name, Category (TreeFruit|Vine|Berry|Vegetable|RowCrop), Description,
DefaultGrowthStages (JSON), IconSvgKey, IsActive
```

#### CropAction
```
Id, CropId, ActionType (Spray|Fertilize|Till|Groom|Clean|Irrigate|Harvest),
DisplayName, DefaultRules (JSON), IsEnabled
```

#### SprayReadinessResult
```
Score (0-100), Band (Go|Caution|Marginal|NoGo),
FactorResults [{ Factor, Value, Status, Reason }],
ProductResults [{ ProductType, IsRecommended, Reason }],
EvaluatedAt
```

#### CropAdvisory
```
CropId, ActionType, Date, LocationId, Status (Recommended|Acceptable|Caution|NotRecommended),
Reasoning, GrowthStage, AiNarrative (nullable), AiGeneratedAt
```

#### UserFarmProfile (Phase 2 — Auth)
```
Id, UserId, FarmName, ActiveCrops [CropId], SavedLocations [LocationId], Settings (JSON)
```

### 4.4 Services

| Service | Responsibility |
|---|---|
| WeatherService | Fetch + cache current weather and forecast from Open-Meteo |
| GeocodingService | ZIP/city autocomplete and lat/lon resolution |
| LocationService | Manage saved locations, browser geolocation interop |
| SprayRuleEngine | Evaluate WeatherSnapshot against spray rules; return SprayReadinessResult |
| CropAdvisoryService | Apply crop+action rules against weather; return CropAdvisory list |
| GrowthStageService | Estimate current phenological stage from crop + date + location |
| AiAdvisoryService | Build prompt, call AI API, parse and return narrative |
| CalendarService | Generate seasonal calendar with stages and suggested actions |

### 4.5 Weather API — Open-Meteo

Base URL: https://api.open-meteo.com/v1/forecast

Key parameters used:
- current: temperature_2m, relative_humidity_2m, apparent_temperature, precipitation,
           weather_code, wind_speed_10m, wind_direction_10m, wind_gusts_10m,
           surface_pressure, visibility, uv_index, dew_point_2m
- hourly: temperature_2m, precipitation_probability, precipitation, wind_speed_10m, wind_gusts_10m
- daily: sunrise, sunset, temperature_2m_max, temperature_2m_min, precipitation_sum
- wind_speed_unit: mph
- temperature_unit: fahrenheit
- precipitation_unit: inch

Geocoding URL: https://geocoding-api.open-meteo.com/v1/search?name={query}&count=5

### 4.6 AI Advisory API

Default: OpenAI Chat Completions
- Model: gpt-4o (configurable)
- Endpoint: https://api.openai.com/v1/chat/completions
- System prompt: "You are an expert agricultural advisor specializing in {crop} farming."
- User prompt: constructed from weather snapshot + crop + growth stage + any failing conditions
- Fallback: GitHub Models endpoint (https://models.inference.ai.azure.com) using same OpenAI SDK

---

## 5. UI/UX Design Principles

- Mobile-first responsive layout (Bootstrap 5 grid)
- Dark/light mode toggle stored in localStorage
- Dashboard page is the landing page; no login required for basic use
- Location auto-detected on first load with fallback to last-used saved location
- All weather values shown in US customary units by default (F, mph, inches); metric toggle available
- Crop cards on dashboard show quick summary; click through to full detail page
- Color system: green (safe/go), yellow (caution), orange (marginal), red (no-go) used consistently
- Loading skeletons while weather data fetches
- SVG weather animations are subtle and accessible (prefers-reduced-motion respected)

---

## 6. Pages & Navigation

| Page | Route | Description |
|---|---|---|
| Dashboard | / | Weather summary + spray score + quick crop cards |
| Crop Detail | /crop/{id} | Full advisory for one crop: all actions, AI button |
| Crop List | /crops | Grid of all configured crops; add/remove |
| Calendar | /crop/{id}/calendar | Seasonal calendar + checklist for one crop |
| Spray Detail | /spray | Full spray readiness breakdown + product grid |
| Settings | /settings | Location mgmt, AI key, unit preferences, crop config |
| About | /about | App info, spec links, data source credits |

---

## 7. Configuration & Settings

Stored in browser localStorage (no-auth mode) or user profile (auth mode):
- Preferred location
- Saved locations list
- Active crops list
- AI provider + API key
- Unit preference (US / Metric)
- Dark/light mode
- Spray rule overrides per crop (custom wind/temp thresholds)
- Calendar checklist completions (per crop, per year)

---

## 8. Phase Roadmap

### Phase 1 — Core (MVP)
- Dashboard with weather + spray score
- Location picker + geolocation + saved locations
- Open-Meteo weather integration
- Spray rule engine (orchard-spec rules)
- Orchard crop set (apple, pear, cherry, peach, grape)
- Crop action cards (spray, fertilize, till)
- Weather SVG icons
- Basic settings page

### Phase 2 — Full Crop Advisory
- All crops in section 3.5.1
- All action types
- Crop calendar with checklist
- Configurable action rules
- Growth stage estimation

### Phase 3 — AI & Auth
- AI advisory integration (OpenAI / GitHub Models)
- ASP.NET Core Identity for multi-user farm profiles
- Server-side storage for profiles + checklist history
- Push notifications (browser) for spray window alerts

### Phase 4 — Advanced
- Historical spray log
- Degree-day pest models
- Equipment maintenance checklist
- Export to PDF / print calendar
- Progressive Web App (PWA) support for offline use
