using System.Text.Json;
using FoodDrinkApp.Models;

namespace FoodDrinkApp.Services;

/// <summary>
/// Loads seed recipes from the embedded recipes.json Maui asset.
/// </summary>
public static class LocalRecipeLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<IReadOnlyList<FoodItem>> LoadFromEmbeddedJsonAsync()
    {
        if (MockApiOptions.SimulateLocalFailureForDemo)
        {
            throw new InvalidOperationException("Simulated local JSON load failure (demo mode).");
        }

        await using var stream = await FileSystem.OpenAppPackageFileAsync("recipes.json");
        using var reader = new StreamReader(stream);
        var json = await reader.ReadToEndAsync();

        var items = JsonSerializer.Deserialize<List<FoodItem>>(json, JsonOptions)
            ?? throw new InvalidOperationException("recipes.json could not be parsed.");

        if (items.Count == 0)
        {
            throw new InvalidOperationException("recipes.json contains no recipes.");
        }

        return items;
    }
}
