using HabitManager.App.ViewModels;
using HabitManager.Domain;

namespace HabitManager.App.Views;

public partial class HabitEditPage : ContentPage
{
    private readonly HabitEditViewModel _viewModel;

    public HabitEditPage(Habit habit)
    {
        InitializeComponent();

        _viewModel = new HabitEditViewModel(habit);
        BindingContext = _viewModel;
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        bool saved = _viewModel.Save();

        if (saved)
        {
            await Navigation.PopAsync();
        }
    }
}