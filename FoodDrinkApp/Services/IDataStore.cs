using FoodDrinkApp.Models;

namespace FoodDrinkApp.Services;

/// <summary>
/// Abstraction for recipe data access, search, favorites, and offline cache.
/// </summary>
public interface IDataStore
{
    /// <summary>Initializes the database and seeds data on first launch.</summary>
    Task InitializeAsync();

    /// <summary>Returns all recipes, optionally simulating a network refresh.</summary>
    Task<IReadOnlyList<FoodItem>> GetRecipesAsync(bool forceRefresh = false);

    /// <summary>Returns a single recipe by identifier.</summary>
    Task<FoodItem?> GetRecipeAsync(string id);

    /// <summary>Searches recipes by name, description, or category.</summary>
    Task<IReadOnlyList<FoodItem>> SearchRecipesAsync(string query);

    /// <summary>Toggles the favorite flag for a recipe.</summary>
    Task ToggleFavoriteAsync(string id);

    /// <summary>Returns recipes recommended for the given coordinates.</summary>
    Task<IReadOnlyList<FoodItem>> GetNearbyRecommendationsAsync(double latitude, double longitude);
}
