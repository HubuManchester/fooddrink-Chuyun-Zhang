using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FoodDrinkApp.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    protected async Task RunSafeAsync(Func<Task> action, string friendlyErrorPrefix)
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            StatusMessage = string.Empty;
            await action();
        }
        catch (Exception ex)
        {
            StatusMessage = $"{friendlyErrorPrefix}: {GetFriendlyMessage(ex)}";
            await ShowAlertAsync("Something went wrong", StatusMessage);
        }
        finally
        {
            IsBusy = false;
        }
    }

    protected static string GetFriendlyMessage(Exception ex) => ex switch
    {
        PermissionException => "Permission was denied. Please enable the feature in device settings and try again.",
        FeatureNotSupportedException => "This feature is not available on the current device.",
        ArgumentException argumentException => argumentException.Message,
        KeyNotFoundException => "The requested item could not be found.",
        _ => "Please try again. If the problem continues, restart the app."
    };

    protected static async Task ShowAlertAsync(string title, string message)
    {
        if (Shell.Current is null)
        {
            return;
        }

        await Shell.Current.DisplayAlert(title, message, "OK");
    }
}
