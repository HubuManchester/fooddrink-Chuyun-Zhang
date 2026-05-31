using FoodDrinkApp.ViewModels;

namespace FoodDrinkApp.Views;

public partial class AddEditRecipePage : ContentPage
{
    public AddEditRecipePage(AddEditRecipeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
