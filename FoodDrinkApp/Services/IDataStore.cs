using FoodDrinkApp.Models;

namespace FoodDrinkApp.Services;

public interface IDataStore
{
    Task InitializeAsync();
    Task<IReadOnlyList<FoodItem>> GetRecipesAsync(bool forceRefresh = false);
    Task<FoodItem?> GetRecipeAsync(string id);
    Task<IReadOnlyList<FoodItem>> SearchRecipesAsync(string query);
    Task ToggleFavoriteAsync(string id);
    Task<IReadOnlyList<FoodItem>> GetNearbyRecommendationsAsync(double latitude, double longitude);
}
