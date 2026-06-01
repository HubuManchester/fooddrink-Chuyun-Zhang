using FoodDrinkApp.Helpers;
using FoodDrinkApp.ViewModels;

namespace FoodDrinkApp.Views;

public partial class AddEditRecipePage : ThemedContentPage
{
    public AddEditRecipePage(AddEditRecipeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
