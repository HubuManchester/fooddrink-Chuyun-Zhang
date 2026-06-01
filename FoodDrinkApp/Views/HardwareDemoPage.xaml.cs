using FoodDrinkApp.Helpers;
using FoodDrinkApp.ViewModels;

namespace FoodDrinkApp.Views;

public partial class HardwareDemoPage : ThemedContentPage
{
    public HardwareDemoPage(HardwareDemoViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
