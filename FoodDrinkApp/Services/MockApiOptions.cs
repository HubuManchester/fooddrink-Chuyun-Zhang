namespace FoodDrinkApp.Services;

/// <summary>
/// Configure your mockapi.io endpoint in <see cref="RecipesEndpoint"/>.
/// </summary>
public static class MockApiOptions
{
    /// <summary>
    /// Example: https://YOUR_PROJECT.mockapi.io/api/v1/recipes
    /// Leave empty to skip HTTP and use built-in seed when API fallback runs.
    /// </summary>
    public static string RecipesEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// When true, embedded JSON load throws so the Mock API catch path can be demonstrated.
    /// </summary>
    public static bool SimulateLocalFailureForDemo { get; set; }
}
