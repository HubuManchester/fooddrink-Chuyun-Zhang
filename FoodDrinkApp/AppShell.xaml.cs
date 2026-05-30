using FoodDrinkApp.Helpers;
using FoodDrinkApp.Views;

namespace FoodDrinkApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(NavigationRoutes.RecipeDetail, typeof(RecipeDetailPage));
    }
}
