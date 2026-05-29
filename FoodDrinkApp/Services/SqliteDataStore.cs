using FoodDrinkApp.Models;
using SQLite;

namespace FoodDrinkApp.Services;

/// <summary>
/// SQLite-backed cache for offline recipe access and favorite persistence.
/// </summary>
public class SqliteDataStore : IDataStore
{
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private SQLiteAsyncConnection? _database;
    private Task? _initializeTask;

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

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "fooddrink.db3");
            _database = new SQLiteAsyncConnection(dbPath);
            await _database.CreateTableAsync<FoodItemRecord>();
            return _database;
        }
        finally
        {
            _initLock.Release();
        }
    }

    public Task InitializeAsync() => _initializeTask ??= InitializeCoreAsync();

    private async Task InitializeCoreAsync()
    {
        var db = await GetDatabaseAsync();
        var count = await db.Table<FoodItemRecord>().CountAsync();
        if (count == 0)
        {
            var seedRecords = MockDataStore.GetSeedRecipes()
                .Select(FoodItemRecord.FromModel)
                .ToList();
            await db.InsertAllAsync(seedRecords);
        }
    }

    public async Task<IReadOnlyList<FoodItem>> GetRecipesAsync(bool forceRefresh = false)
    {
        await InitializeAsync();
        var db = await GetDatabaseAsync();

        if (forceRefresh)
        {
            // Simulate a network refresh while keeping favorites intact.
            var favorites = (await db.Table<FoodItemRecord>().Where(r => r.IsFavorite).ToListAsync())
                .ToDictionary(r => r.Id, r => r.IsFavorite);

            await db.DeleteAllAsync<FoodItemRecord>();
            var refreshed = MockDataStore.GetSeedRecipes()
                .Select(item =>
                {
                    if (favorites.TryGetValue(item.Id, out var isFavorite))
                    {
                        item.IsFavorite = isFavorite;
                    }

                    return FoodItemRecord.FromModel(item);
                })
                .ToList();
            await db.InsertAllAsync(refreshed);
        }

        var records = await db.Table<FoodItemRecord>().OrderBy(r => r.Name).ToListAsync();
        return records.Select(r => r.ToModel()).ToList();
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

        await InitializeAsync();
        var db = await GetDatabaseAsync();
        var normalized = query.Trim().ToLowerInvariant();
        var records = await db.Table<FoodItemRecord>().ToListAsync();

        return records
            .Where(r =>
                r.Name.Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
                r.Description.Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
                r.Category.Contains(normalized, StringComparison.OrdinalIgnoreCase))
            .Select(r => r.ToModel())
            .ToList();
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
        await InitializeAsync();
        var region = ResolveRegion(latitude, longitude);
        var recipes = await GetRecipesAsync();

        return recipes
            .Where(r => string.Equals(r.Region, region, StringComparison.OrdinalIgnoreCase))
            .Take(3)
            .ToList();
    }

    /// <summary>
    /// Maps coarse GPS coordinates to a recipe region for demo recommendations.
    /// </summary>
    private static string ResolveRegion(double latitude, double longitude)
    {
        if (latitude is >= 35 and <= 70 && longitude is >= -10 and <= 40)
        {
            return "Europe";
        }

        if (latitude is >= -10 and <= 45 && longitude is >= 60 and <= 150)
        {
            return "Asia";
        }

        if (latitude is >= -55 and <= 25 && longitude is >= -120 and <= -30)
        {
            return "Americas";
        }

        if (latitude is >= 30 and <= 45 && longitude is >= -10 and <= 35)
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
        public string Steps { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public bool IsFavorite { get; set; }

        public static FoodItemRecord FromModel(FoodItem item) => new()
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Emoji = item.Emoji,
            Calories = item.Calories,
            Steps = item.Steps,
            Category = item.Category,
            Region = item.Region,
            IsFavorite = item.IsFavorite
        };

        public FoodItem ToModel() => new()
        {
            Id = Id,
            Name = Name,
            Description = Description,
            Emoji = Emoji,
            Calories = Calories,
            Steps = Steps,
            Category = Category,
            Region = Region,
            IsFavorite = IsFavorite
        };
    }
}
