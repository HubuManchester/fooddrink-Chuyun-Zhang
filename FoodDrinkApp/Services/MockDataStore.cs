using FoodDrinkApp.Models;

namespace FoodDrinkApp.Services;

/// <summary>
/// Seed data used on first launch and when refreshing from the network fails.
/// </summary>
public static class MockDataStore
{
    public static IReadOnlyList<FoodItem> GetSeedRecipes() =>
    [
        new FoodItem
        {
            Id = "1",
            Name = "Mediterranean Salad Bowl",
            Description = "Fresh vegetables, olives, and feta with lemon dressing.",
            Emoji = "🥗",
            Calories = 320,
            Category = "Salad",
            Region = "Mediterranean",
            Steps = "1. Chop cucumber, tomato, and red onion.\n2. Add olives and feta.\n3. Drizzle olive oil and lemon juice.\n4. Toss and serve chilled."
        },
        new FoodItem
        {
            Id = "2",
            Name = "Classic Spaghetti Carbonara",
            Description = "Creamy Italian pasta with pancetta and parmesan.",
            Emoji = "🍝",
            Calories = 540,
            Category = "Pasta",
            Region = "Europe",
            Steps = "1. Boil spaghetti until al dente.\n2. Fry pancetta until crisp.\n3. Mix eggs, parmesan, and pepper.\n4. Combine pasta, pancetta, and egg mixture off heat."
        },
        new FoodItem
        {
            Id = "3",
            Name = "Chicken Teriyaki Rice",
            Description = "Glazed chicken served over steamed rice with vegetables.",
            Emoji = "🍱",
            Calories = 480,
            Category = "Main",
            Region = "Asia",
            Steps = "1. Marinate chicken in teriyaki sauce.\n2. Pan-fry until cooked through.\n3. Steam rice and blanch broccoli.\n4. Plate rice, vegetables, and sliced chicken."
        },
        new FoodItem
        {
            Id = "4",
            Name = "Berry Smoothie",
            Description = "Blended berries, banana, and yogurt for a quick breakfast.",
            Emoji = "🥤",
            Calories = 210,
            Category = "Drink",
            Region = "Americas",
            Steps = "1. Add berries, banana, and yogurt to a blender.\n2. Add a splash of milk or water.\n3. Blend until smooth.\n4. Serve immediately over ice."
        },
        new FoodItem
        {
            Id = "5",
            Name = "Vegetable Curry",
            Description = "Aromatic coconut curry with seasonal vegetables.",
            Emoji = "🍛",
            Calories = 390,
            Category = "Curry",
            Region = "Asia",
            Steps = "1. Sauté onion, garlic, and ginger.\n2. Stir in curry paste and coconut milk.\n3. Simmer vegetables until tender.\n4. Garnish with coriander and serve with rice."
        },
        new FoodItem
        {
            Id = "6",
            Name = "Avocado Toast",
            Description = "Wholegrain toast topped with smashed avocado and chili flakes.",
            Emoji = "🥑",
            Calories = 280,
            Category = "Breakfast",
            Region = "Americas",
            Steps = "1. Toast bread until golden.\n2. Mash avocado with lime and salt.\n3. Spread on toast.\n4. Top with chili flakes and optional egg."
        }
    ];
}
