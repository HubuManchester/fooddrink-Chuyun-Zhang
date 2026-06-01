using FoodDrinkApp.Helpers;
using FoodDrinkApp.ViewModels;

namespace FoodDrinkApp.Views;

public partial class SettingsPage : ThemedContentPage
{
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
