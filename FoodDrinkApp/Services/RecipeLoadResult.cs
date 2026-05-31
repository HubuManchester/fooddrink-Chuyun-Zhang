using FoodDrinkApp.Models;

namespace FoodDrinkApp.Services;

/// <summary>
/// Result of a recipe load operation, including the data source used.
/// </summary>
public sealed class RecipeLoadResult
{
    public RecipeLoadResult(IReadOnlyList<FoodItem> recipes, string dataSource)
    {
        Recipes = recipes;
        DataSource = dataSource;
    }

    public IReadOnlyList<FoodItem> Recipes { get; }

    /// <summary>Human-readable source, e.g. SQLite, recipes.json, or Mock API.</summary>
    public string DataSource { get; }
}
