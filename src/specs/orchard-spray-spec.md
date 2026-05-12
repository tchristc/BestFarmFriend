# Orchard Spray Specification

## Overview
This document defines the rules, conditions, and logic for determining whether a given day is suitable
for spraying orchard crops (fruit trees, nut trees, berry bushes, and vines).
These rules were provided by an experienced orchard operator and form the core domain logic for the
BestFarmFriend spray-advisory engine.

---

## 1. Spray Window — General Rules

### 1.1 Wind
| Condition | Result |
|---|---|
| Wind speed <= 5 mph | Ideal |
| Wind speed 6-10 mph | Marginal — short window spraying only |
| Wind speed > 10 mph | Do NOT spray |
| Gusts > 15 mph (even if avg <= 10) | Do NOT spray |

### 1.2 Rain / Precipitation
| Condition | Result |
|---|---|
| No rain in forecast next 4 hours | OK |
| Rain expected within 1-4 hours | Marginal — systemic products only |
| Rain expected within 1 hour | Do NOT spray |
| Rain in the past 2 hours (wet foliage) | Do NOT spray |
| Cumulative rain past 24h > 0.5 in | Check soil saturation before entering orchard with equipment |

### 1.3 Temperature
| Condition | Result |
|---|---|
| 45F - 90F | OK |
| < 45F | Marginal — many pesticides lose efficacy; no copper below 40F |
| < 40F | Do NOT spray fungicides or copper products |
| > 90F | Avoid midday; spray early morning or evening only |
| > 95F | Do NOT spray — phytotoxicity risk |

### 1.4 Humidity / Dew Point
| Condition | Result |
|---|---|
| Relative Humidity < 85% | OK |
| RH 85-95% | Marginal — use caution; disease pressure high |
| RH > 95% | Do NOT spray — foliage already wet or near-wet; absorption risk |
| Dew Point within 3F of Air Temp | Dew likely; delay or spray only if product requires moist uptake |

### 1.5 Time of Day
| Condition | Result |
|---|---|
| 1 hour after sunrise - 2 hours before sunset | Ideal spray window |
| Within 1 hour of sunrise | Dew may still be present |
| Within 2 hours of sunset | Dew may form; fungal pressure increases |
| Night (after sunset, before sunrise) | Avoid — poor coverage visibility, dew risk |

---

## 2. Product-Specific Overrides

### 2.1 Copper-Based Fungicides (e.g., Copper Hydroxide, Bordeaux Mix)
- Minimum temperature: 40F
- Do NOT apply if frost forecast within 24 hours
- Do NOT apply to wet foliage (rain within 2 hours)
- Do NOT apply if RH > 90%
- Re-entry interval: 24 hours
- Rain-fast after: 2 hours dry time required

### 2.2 Sulfur-Based Fungicides
- Do NOT apply when temps exceed 90F (phytotoxicity risk)
- Do NOT apply within 2 weeks of applying oil-based products
- Minimum temperature: 50F for efficacy
- Rain-fast after: 1 hour

### 2.3 Oil-Based Sprays (Dormant / Horticultural Oil)
- Apply during dormant season only (before bud break or in late fall) unless labeled for growing season
- Temperature must be 32F-85F during application AND for 24 hours after
- Do NOT apply if frost forecast within 24 hours
- Do NOT apply within 2 weeks of sulfur-based product
- Rain-fast after: 24 hours (must remain on bark)

### 2.4 Insecticides (General)
- Follow general wind/rain/temp rules above
- Do NOT apply during bloom (risk to pollinators)
- Preferred application time: early morning or late evening to reduce bee exposure
- If beneficial insect populations are present and active, delay until they are less active

### 2.5 Herbicides (Under-Tree / Row Strips)
- Wind <= 5 mph strictly (drift onto foliage is damaging)
- Use shielded/directed sprayers when wind is 3-5 mph
- Do NOT apply if rain expected within 24 hours (pre-emergent) or 4 hours (post-emergent)
- Temperature: 50F-85F for best activity

---

## 3. Spray Readiness Score

A composite 0-100 score derived from weighted conditions:

| Factor | Weight |
|---|---|
| Wind Speed | 30% |
| Precipitation (past + forecast) | 25% |
| Temperature | 20% |
| Humidity / RH | 15% |
| Time of Day | 10% |

Interpretation:
- 80-100: Go — Excellent spray day
- 60-79:  Caution — Acceptable with awareness
- 40-59:  Marginal — Delay if possible
- 0-39:   No-Go — Do not spray today

---

## 4. Orchard Crop Types Covered

| Crop | Key Notes |
|---|---|
| Apple | High copper/sulfur use; bloom protection critical |
| Pear | Similar to apple; fire blight pressure drives copper timing |
| Cherry (Sweet & Sour) | Avoid copper near harvest; rain-cracking concern |
| Peach / Nectarine | Brown rot pressure; sulfur timing critical |
| Plum / Prune | Black knot; copper important in dormant season |
| Apricot | Short bloom window; very temperature sensitive |
| Quince | Similar to apple/pear |
| Fig | Minimal spray; mainly scale & rust mite with oil |
| Grape (Wine & Table) | Downy/powdery mildew; sulfur and copper on tight schedule |
| Blueberry | Mummy berry; copper at bud swell |
| Raspberry / Blackberry | Cane blight; copper in fall |
| Strawberry | Botrytis pressure; captan timing |
| Walnut | Codling moth + walnut blight; kaolin clay useful |
| Almond | Bloom spray critical; hull rot late season |
| Hazelnut | Eastern Filbert Blight (copper) |
| Pecan | Scab pressure; zinc sulfate important |

---

## 5. Phenological Growth Stage Modifiers

Spray rules and product restrictions change by growth stage:

| Stage | Code | Key Actions |
|---|---|---|
| Dormant | DOR | Dormant oil, copper |
| Silver Tip | ST | First copper app |
| Green Tip | GT | Copper + captan |
| Half-Inch Green | HIG | Copper/captan; watch temp |
| Tight Cluster | TC | Copper/captan/sulfur |
| Pink / Popcorn | PP | Last pre-bloom; insecticide window |
| Full Bloom | BLM | NO INSECTICIDES; copper if fire blight risk |
| Petal Fall | PF | Resume full program |
| Fruit Set | FS | Thin + spray resume |
| First Cover | FC | Full spray program |
| Second Cover onward | SC+ | Regular cover spray schedule |
| Pre-Harvest | PH | Check PHI for all products; no copper within 30 days |
| Post-Harvest | POST | Copper for leaf curl prevention |

---

## 6. Spray Calendar — Apple Example

| Month | Stage | Recommended Products | Notes |
|---|---|---|---|
| Feb | Dormant | Dormant oil | Temps must stay above 32F for 24h |
| Mar | Silver Tip - Green Tip | Copper | Watch frost; no bloom |
| Mar-Apr | Tight Cluster - Pink | Copper + Captan + Mancozeb | Key fire blight window |
| Apr | Full Bloom | Copper only if fire blight forecast | NO insecticides |
| Apr-May | Petal Fall - Fruit Set | Full program restarts | Thin fruit; cover spray |
| May-Jul | Cover Sprays | Rotate fungicides + insecticide | 7-14 day intervals |
| Aug | Pre-Harvest | Verify PHI | Remove products with long PHI |
| Sep-Oct | Post-Harvest | Copper | Leaf curl & fire blight prevention |

---

## 7. Integration Points for BestFarmFriend App

- WeatherService feeds temperature, wind, humidity, precipitation, forecast into the spray-rule engine
- SprayRuleEngine evaluates each condition against the rules in sections 1-2 and returns a SprayReadinessResult
- CropAdvisoryService applies product-specific overrides (section 2) and growth-stage modifiers (section 5)
- GrowthStageService tracks or estimates current growth stage from calendar date + location + historical climate data
- AI Advisory sends current weather + crop + growth stage context to the configured AI API (GitHub Copilot / OpenAI) for a narrative recommendation
- Dashboard displays SprayReadinessScore with color coding, individual factor breakdowns, and crop-specific notes

---

## 8. Data Sources

| Data | Source |
|---|---|
| Current weather | Open-Meteo API (free, no key required) or OpenWeatherMap |
| Forecast (hourly) | Open-Meteo hourly forecast endpoint |
| Location / ZIP lookup | Open-Meteo geocoding API or Nominatim (OpenStreetMap) |
| Growth stage estimation | Rule-based calendar + user input |
| AI narrative | OpenAI API (GPT-4o) or GitHub Models endpoint |
