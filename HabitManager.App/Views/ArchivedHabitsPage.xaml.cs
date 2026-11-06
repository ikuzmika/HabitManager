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

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        Content.IsEnabled = false;

        try
        {
            await _viewModel.LoadAsync();
        }
        catch (Exception exception)
        {
            await DisplayAlertAsync(
                "Unable to load archived habits",
                exception.Message,
                "OK");
        }
        finally
        {
            Content.IsEnabled = true;
        }
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
                "Could not restore the habit",
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