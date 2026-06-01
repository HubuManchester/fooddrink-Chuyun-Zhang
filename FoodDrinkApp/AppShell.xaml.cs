using FoodDrinkApp.Helpers;
using FoodDrinkApp.Views;

namespace FoodDrinkApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(NavigationRoutes.RecipeDetail, typeof(RecipeDetailPage));
        Routing.RegisterRoute(NavigationRoutes.AddEditRecipe, typeof(AddEditRecipePage));
        Routing.RegisterRoute(NavigationRoutes.CameraCapture, typeof(CameraCapturePage));
    }
}
