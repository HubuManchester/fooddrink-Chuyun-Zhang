using FoodDrinkApp.ViewModels;

namespace FoodDrinkApp.Views;

public partial class HardwareDemoPage : ContentPage
{
    public HardwareDemoPage(HardwareDemoViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
