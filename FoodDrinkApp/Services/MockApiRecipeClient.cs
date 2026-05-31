using System.Text.Json;
using FoodDrinkApp.Models;

namespace FoodDrinkApp.Services;

/// <summary>
/// Fetches recipes from a mockapi.io REST endpoint when local data is unavailable.
/// </summary>
public class MockApiRecipeClient
{
    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public MockApiRecipeClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(15);
    }

    public async Task<IReadOnlyList<FoodItem>> FetchRecipesAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(MockApiOptions.RecipesEndpoint))
        {
            throw new InvalidOperationException(
                "Mock API URL is not configured. Set MockApiOptions.RecipesEndpoint to your mockapi.io recipes URL.");
        }

        var json = await _httpClient.GetStringAsync(MockApiOptions.RecipesEndpoint, cancellationToken);
        var dtos = JsonSerializer.Deserialize<List<MockApiRecipeDto>>(json, JsonOptions)
            ?? throw new InvalidOperationException("Mock API returned invalid JSON.");

        return dtos
            .Where(d => !string.IsNullOrWhiteSpace(d.Name))
            .Select(d => d.ToFoodItem())
            .ToList();
    }

    private sealed class MockApiRecipeDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Emoji { get; set; }
        public int Calories { get; set; }
        public string? Category { get; set; }
        public string? Region { get; set; }
        public string? Ingredients { get; set; }
        public string? Steps { get; set; }

        public FoodItem ToFoodItem() => new()
        {
            Id = string.IsNullOrWhiteSpace(Id) ? Guid.NewGuid().ToString("N") : Id!,
            Name = Name ?? "Untitled recipe",
            Description = Description ?? string.Empty,
            Emoji = string.IsNullOrWhiteSpace(Emoji) ? "🍽️" : Emoji,
            Calories = Calories,
            Category = string.IsNullOrWhiteSpace(Category) ? "Main" : Category,
            Region = string.IsNullOrWhiteSpace(Region) ? "Europe" : Region,
            Ingredients = Ingredients ?? string.Empty,
            Steps = Steps ?? "No steps provided."
        };
    }
}
