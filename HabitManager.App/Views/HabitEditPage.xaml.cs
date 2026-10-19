using HabitManager.App.ViewModels;
using HabitManager.Domain;

namespace HabitManager.App.Views;

public partial class HabitEditPage : ContentPage
{
    private readonly HabitEditViewModel _viewModel;

    private bool _isSaving;

    public HabitEditPage(Habit habit)
    {
        InitializeComponent();

        _viewModel = new HabitEditViewModel(habit, App.HabitService);
        BindingContext = _viewModel;
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (_isSaving)
        {
            return;
        }

        _isSaving = true;
        Content.IsEnabled = false;

        bool saved;

        try
        {
            saved = await _viewModel.SaveAsync();
        }
        finally
        {
            _isSaving = false;
            Content.IsEnabled = true;
        }

        if (saved)
        {
            await Navigation.PopAsync();
        }
    }
}