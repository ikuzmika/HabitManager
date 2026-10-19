using HabitManager.App.ViewModels;
using HabitManager.Domain;

namespace HabitManager.App.Views;

public partial class HabitCreatePage : ContentPage
{
    private readonly HabitCreateViewModel _viewModel;
    private readonly HabitListViewModel _listViewModel;
    private bool _created;

    public HabitCreatePage(HabitListViewModel listViewModel)
    {
        InitializeComponent();

        _listViewModel = listViewModel;
        _viewModel = new HabitCreateViewModel("demo-user-1");

        BindingContext = _viewModel;
    }

    private async void OnCreateClicked(object? sender, EventArgs e)
    {
        if (_created)
        {
            return;
        }

        Habit? habit = _viewModel.Create();

        if (habit == null)
        {
            return;
        }

        _created = true;

        _listViewModel.AddHabit(habit);

        await Navigation.PopAsync();
    }
}