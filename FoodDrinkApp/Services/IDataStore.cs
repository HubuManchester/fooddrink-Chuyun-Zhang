using FoodDrinkApp.Models;

namespace FoodDrinkApp.Services;

/// <summary>
/// Abstraction for recipe data access, search, favorites, and offline cache.
/// </summary>
public interface IDataStore
{
    /// <summary>Loads recipes: local JSON/SQLite first; Mock API on failure (see implementation).</summary>
    Task<RecipeLoadResult> LoadRecipesAsync(bool forceRefresh = false);

    Task<IReadOnlyList<FoodItem>> GetRecipesAsync(bool forceRefresh = false);

    Task<FoodItem?> GetRecipeAsync(string id);

    Task<IReadOnlyList<FoodItem>> SearchRecipesAsync(string query);

    Task<IReadOnlyList<FoodItem>> GetRecipesByCategoryAsync(string? category);

    Task ToggleFavoriteAsync(string id);

    Task AddRecipeAsync(FoodItem item);

    Task UpdateRecipeAsync(FoodItem item);

    Task DeleteRecipeAsync(string id);

    Task<IReadOnlyList<FoodItem>> GetNearbyRecommendationsAsync(double latitude, double longitude);

    IReadOnlyList<string> GetCategories();
}
