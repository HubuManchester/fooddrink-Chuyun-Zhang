using FoodDrinkApp.Helpers;
using FoodDrinkApp.ViewModels;

namespace FoodDrinkApp.Views;

public partial class RecipeDetailPage : ThemedContentPage
{
    public RecipeDetailPage(RecipeDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
