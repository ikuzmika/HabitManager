using HabitManager.App.ViewModels;
using HabitManager.Domain;

namespace HabitManager.App.Views;

public partial class HabitDetailsPage : ContentPage
{
    public HabitDetailsPage(Habit habit)
    {
        InitializeComponent();
        
        BindingContext = new HabitDetailsViewModel(habit, App.HabitService);
    }
    
    private async void OnEditClicked(object? sender, EventArgs e)
    {
        if (BindingContext is not HabitDetailsViewModel viewModel)
        {
            return;
        }

        if (!viewModel.CanEdit)
        {
            return;
        }

        await Navigation.PushAsync(
            new HabitEditPage(viewModel.SelectedHabit));
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is HabitDetailsViewModel viewModel)
        {
            viewModel.Refresh();
        }
    }
}