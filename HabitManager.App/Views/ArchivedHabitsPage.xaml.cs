using HabitManager.App.ViewModels;
using HabitManager.Domain;

namespace HabitManager.App.Views;

public partial class ArchivedHabitsPage : ContentPage
{
    private readonly ArchivedHabitsViewModel _viewModel;
    private bool _isRestoring;

    public ArchivedHabitsPage()
    {
        InitializeComponent();

        _viewModel = new ArchivedHabitsViewModel(
            App.HabitService);

        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _viewModel.Refresh();
    }

    private async void OnRestoreClicked(object? sender, EventArgs e)
    {
        if (_isRestoring)
        {
            return;
        }

        if (sender is not Button button ||
            button.CommandParameter is not Habit habit)
        {
            return;
        }

        _isRestoring = true;
        Content.IsEnabled = false;

        try
        {
            await _viewModel.UnarchiveAsync(habit);
        }
        catch (Exception exception)
        {
            await DisplayAlertAsync(
                "Could not save changes",
                "The habit may be active in memory, but the change " +
                "was not saved. Try Restore again. " +
                exception.Message,
                "OK");
        }
        finally
        {
            _isRestoring = false;
            Content.IsEnabled = true;
        }
    }
}