using FoodDrinkApp.ViewModels;

namespace FoodDrinkApp.Views;

public partial class HomePage : ContentPage
{
    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is HomeViewModel viewModel)
        {
            _ = viewModel.LoadRecipesCommand.ExecuteAsync(null);
        }
    }
}
