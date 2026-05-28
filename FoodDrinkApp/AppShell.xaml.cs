using FoodDrinkApp.Views;

namespace FoodDrinkApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(RecipeDetailPage), typeof(RecipeDetailPage));
    }
}
