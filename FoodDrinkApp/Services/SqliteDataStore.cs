using FoodDrinkApp.Models;
using SQLite;

namespace FoodDrinkApp.Services;

/// <summary>
/// SQLite cache with local-first loading and Mock API fallback.
/// </summary>
public class SqliteDataStore : IDataStore
{
    private const string DatabaseFileName = "fooddrink_v3.db3";

    private readonly MockApiRecipeClient _mockApiClient;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private SQLiteAsyncConnection? _database;
    private Task? _initializeTask;

    private static readonly string[] Categories =
        ["All", "Breakfast", "Salad", "Pasta", "Main", "Drink", "Curry"];

    public SqliteDataStore(MockApiRecipeClient mockApiClient)
    {
        _mockApiClient = mockApiClient;
    }

    public IReadOnlyList<string> GetCategories() => Categories;

    public Task InitializeAsync() => _initializeTask ??= InitializeCoreAsync();

    private async Task InitializeCoreAsync()
    {
        _ = await GetDatabaseAsync();
    }

    /// <inheritdoc />
    public async Task<RecipeLoadResult> LoadRecipesAsync(bool forceRefresh = false)
    {
        await InitializeAsync();

        if (forceRefresh)
        {
            try
            {
                var apiRecipes = await _mockApiClient.FetchRecipesAsync();
                await ReplaceAllInDatabaseAsync(apiRecipes, preserveFavorites: true);
                return new RecipeLoadResult(apiRecipes, "Mock API (pull-to-refresh)");
            }
            catch
            {
                // Fall through to local reload on refresh failure.
            }
        }
        else
        {
            var cached = await ReadAllFromDatabaseAsync();
            if (cached.Count > 0)
            {
                return new RecipeLoadResult(cached, "SQLite local cache");
            }
        }

        // Local-first path: embedded JSON → SQLite.
        try
        {
            var fromJson = await LocalRecipeLoader.LoadFromEmbeddedJsonAsync();
            await ReplaceAllInDatabaseAsync(fromJson, preserveFavorites: forceRefresh);
            return new RecipeLoadResult(fromJson, "recipes.json (embedded resource)");
        }
        catch (Exception)
        {
            // Local failed — try Mock API (dual-insurance requirement).
            try
            {
                var fromApi = await _mockApiClient.FetchRecipesAsync();
                await ReplaceAllInDatabaseAsync(fromApi, preserveFavorites: true);
                return new RecipeLoadResult(fromApi, "Mock API (local load failed)");
            }
            catch
            {
                var seed = MockDataStore.GetSeedRecipes().ToList();
                await ReplaceAllInDatabaseAsync(seed, preserveFavorites: true);
                return new RecipeLoadResult(seed, "Built-in seed (local and Mock API unavailable)");
            }
        }
    }

    public async Task<IReadOnlyList<FoodItem>> GetRecipesAsync(bool forceRefresh = false)
    {
        var result = await LoadRecipesAsync(forceRefresh);
        return result.Recipes;
    }

    public async Task<FoodItem?> GetRecipeAsync(string id)
    {
        await InitializeAsync();
        var db = await GetDatabaseAsync();
        var record = await db.FindAsync<FoodItemRecord>(id);
        return record?.ToModel();
    }

    public async Task<IReadOnlyList<FoodItem>> SearchRecipesAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException("Search text cannot be empty.", nameof(query));
        }

        var recipes = await GetRecipesAsync();
        var normalized = query.Trim();

        return recipes
            .Where(r =>
                r.Name.Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
                r.Description.Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
                r.Category.Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
                r.Ingredients.Contains(normalized, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public async Task<IReadOnlyList<FoodItem>> GetRecipesByCategoryAsync(string? category)
    {
        var recipes = await GetRecipesAsync();
        if (string.IsNullOrWhiteSpace(category) || category == "All")
        {
            return recipes;
        }

        return recipes
            .Where(r => string.Equals(r.Category, category, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public async Task AddRecipeAsync(FoodItem item)
    {
        if (string.IsNullOrWhiteSpace(item.Name))
        {
            throw new ArgumentException("Recipe name is required.", nameof(item));
        }

        await InitializeAsync();
        var db = await GetDatabaseAsync();

        if (string.IsNullOrWhiteSpace(item.Id))
        {
            item.Id = Guid.NewGuid().ToString("N");
        }

        await db.InsertAsync(FoodItemRecord.FromModel(item));
    }

    public async Task UpdateRecipeAsync(FoodItem item)
    {
        if (string.IsNullOrWhiteSpace(item.Id))
        {
            throw new ArgumentException("Recipe id is required.", nameof(item));
        }

        await InitializeAsync();
        var db = await GetDatabaseAsync();
        var rows = await db.UpdateAsync(FoodItemRecord.FromModel(item));
        if (rows == 0)
        {
            throw new KeyNotFoundException($"Recipe '{item.Id}' was not found.");
        }
    }

    public async Task DeleteRecipeAsync(string id)
    {
        await InitializeAsync();
        var db = await GetDatabaseAsync();
        var rows = await db.DeleteAsync<FoodItemRecord>(id);
        if (rows == 0)
        {
            throw new KeyNotFoundException($"Recipe '{id}' was not found.");
        }
    }

    public async Task ToggleFavoriteAsync(string id)
    {
        await InitializeAsync();
        var db = await GetDatabaseAsync();
        var record = await db.FindAsync<FoodItemRecord>(id)
            ?? throw new KeyNotFoundException($"Recipe '{id}' was not found.");

        record.IsFavorite = !record.IsFavorite;
        await db.UpdateAsync(record);
    }

    public async Task<IReadOnlyList<FoodItem>> GetNearbyRecommendationsAsync(double latitude, double longitude)
    {
        var region = ResolveRegion(latitude, longitude);
        var recipes = await GetRecipesAsync();

        return recipes
            .Where(r => string.Equals(r.Region, region, StringComparison.OrdinalIgnoreCase))
            .Take(3)
            .ToList();
    }

    private async Task<SQLiteAsyncConnection> GetDatabaseAsync()
    {
        if (_database is not null)
        {
            return _database;
        }

        await _initLock.WaitAsync();
        try
        {
            if (_database is not null)
            {
                return _database;
            }

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName);
            _database = new SQLiteAsyncConnection(dbPath);
            await _database.CreateTableAsync<FoodItemRecord>();
            return _database;
        }
        finally
        {
            _initLock.Release();
        }
    }

    private async Task<IReadOnlyList<FoodItem>> ReadAllFromDatabaseAsync()
    {
        var db = await GetDatabaseAsync();
        var records = await db.Table<FoodItemRecord>().OrderBy(r => r.Name).ToListAsync();
        return records.Select(r => r.ToModel()).ToList();
    }

    private async Task ReplaceAllInDatabaseAsync(IReadOnlyList<FoodItem> recipes, bool preserveFavorites)
    {
        var db = await GetDatabaseAsync();
        Dictionary<string, bool>? favorites = null;

        if (preserveFavorites)
        {
            favorites = (await db.Table<FoodItemRecord>().Where(r => r.IsFavorite).ToListAsync())
                .ToDictionary(r => r.Id, r => r.IsFavorite);
        }

        await db.DeleteAllAsync<FoodItemRecord>();

        var records = recipes.Select(FoodItemRecord.FromModel).ToList();
        if (favorites is not null)
        {
            foreach (var record in records)
            {
                if (favorites.TryGetValue(record.Id, out var isFavorite))
                {
                    record.IsFavorite = isFavorite;
                }
            }
        }

        await db.InsertAllAsync(records);
    }

    private static string ResolveRegion(double latitude, double longitude)
    {
        if (latitude >= 35 && latitude <= 70 && longitude >= -10 && longitude <= 40)
        {
            return "Europe";
        }

        if (latitude >= -10 && latitude <= 45 && longitude >= 60 && longitude <= 150)
        {
            return "Asia";
        }

        if (latitude >= -55 && latitude <= 25 && longitude >= -120 && longitude <= -30)
        {
            return "Americas";
        }

        if (latitude >= 30 && latitude <= 45 && longitude >= -10 && longitude <= 35)
        {
            return "Mediterranean";
        }

        return "Europe";
    }

    [Table("FoodItems")]
    private class FoodItemRecord
    {
        [PrimaryKey]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Emoji { get; set; } = string.Empty;
        public int Calories { get; set; }
        public string Ingredients { get; set; } = string.Empty;
        public string Steps { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public bool IsFavorite { get; set; }

        public static FoodItemRecord FromModel(FoodItem item) => new()
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Emoji = item.Emoji,
            Calories = item.Calories,
            Ingredients = item.Ingredients,
            Steps = item.Steps,
            Category = item.Category,
            Region = item.Region,
            ImagePath = item.ImagePath,
            IsFavorite = item.IsFavorite
        };

        public FoodItem ToModel() => new()
        {
            Id = Id,
            Name = Name,
            Description = Description,
            Emoji = Emoji,
            Calories = Calories,
            Ingredients = Ingredients,
            Steps = Steps,
            Category = Category,
            Region = Region,
            ImagePath = ImagePath,
            IsFavorite = IsFavorite
        };
    }
}
