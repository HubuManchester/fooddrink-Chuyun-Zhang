# Data source configuration

The current FoodDrinkApp uses **local seed data** and a **SQLite offline cache** instead of a remote API.

## Current data flow

1. `Services/MockDataStore.cs` provides seed recipes on first launch.
2. `Services/SqliteDataStore.cs` stores recipes and favorites in `fooddrink.db3`.
3. Refresh simulates a network update while preserving favorite flags.

## Optional: mockapi.io integration

If you extend the project to use [mockapi.io](https://mockapi.io), create a `foods` resource with fields such as:

| Field | Type | Description |
|---|---|---|
| name | String | Recipe name |
| category | String | Category |
| description | String | Short description |
| calories | Number | Calories |
| steps | String | Cooking steps |
| region | String | Region for location-based recommendations |

Configure the API URL in a service class and keep local SQLite as a fallback for offline use.
