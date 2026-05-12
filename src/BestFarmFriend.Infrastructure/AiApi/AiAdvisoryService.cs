using BestFarmFriend.Core.Interfaces;
using BestFarmFriend.Core.Models;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using System.ClientModel;

namespace BestFarmFriend.Infrastructure.AiApi;

public class AiAdvisoryService : IAiAdvisoryService
{
    private readonly ILogger<AiAdvisoryService> _logger;
    private string? _apiKey;
    private string _model = "gpt-4o";

    public AiAdvisoryService(ILogger<AiAdvisoryService> logger)
    {
        _logger = logger;
    }

    public void Configure(string apiKey, string model = "gpt-4o")
    {
        _apiKey = apiKey;
        _model = model;
    }

    public async Task<string> GetNarrativeAsync(
        WeatherSnapshot weather,
        Crop crop,
        GrowthStageCode stage,
        SprayReadinessResult sprayResult,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            return "AI advisory is not configured. Please add your OpenAI API key in Settings.";

        string prompt = BuildPrompt(weather, crop, stage, sprayResult);

        try
        {
            var client = new ChatClient(_model, new ApiKeyCredential(_apiKey));

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage($"You are an expert agricultural advisor specializing in {crop.Name} farming. Provide concise, practical, actionable advice in plain text. Keep your response under 300 words."),
                new UserChatMessage(prompt)
            };

            var response = await client.CompleteChatAsync(messages, cancellationToken: ct);
            return response.Value.Content[0].Text;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI advisory call failed for crop {CropName}", crop.Name);
            return $"AI advisory unavailable: {ex.Message}";
        }
    }

    private static string BuildPrompt(WeatherSnapshot w, Crop crop, GrowthStageCode stage, SprayReadinessResult spray)
    {
        var failingConditions = spray.FactorResults
            .Where(f => f.Status != FactorStatus.Pass)
            .Select(f => $"{f.Factor}: {f.Reason}");

        return $"""
            Today is {DateTime.Today:MMMM d, yyyy}.
            Crop: {crop.Name}. Estimated growth stage: {stage}.

            Current weather:
            - Temperature: {w.TemperatureF:F1}°F (feels like {w.FeelsLikeF:F1}°F)
            - Wind: {w.WindSpeedMph:F1} mph, gusting to {w.WindGustMph:F1} mph
            - Humidity: {w.HumidityPct:F0}%, Dew Point: {w.DewPointF:F1}°F
            - Precipitation past 1h: {w.PrecipPast1hIn:F2}", past 24h: {w.PrecipPast24hIn:F2}"
            - Pressure: {w.PressureHpa:F0} hPa ({w.PressureTrend})
            - UV Index: {w.UvIndex:F1}

            Spray Readiness Score: {spray.Score}/100 ({spray.BandLabel})
            Conditions needing attention: {(failingConditions.Any() ? string.Join("; ", failingConditions) : "None")}

            Please provide:
            1. Overall recommendation for today's farming activities
            2. Specific spray advice (what to apply or avoid and why)
            3. Other important actions for {crop.Name} at this growth stage today
            4. What to watch for in the next 24–48 hours
            """;
    }
}
