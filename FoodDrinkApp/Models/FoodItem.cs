namespace FoodDrinkApp.Models;

/// <summary>
/// Represents a recipe / food item shown in the home list and detail views.
/// </summary>
public class FoodItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Emoji { get; set; } = "🍽️";
    public int Calories { get; set; }
    public string Ingredients { get; set; } = string.Empty;
    public string Steps { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public string Region { get; set; } = string.Empty;
    public bool IsFavorite { get; set; }
}
