using System.Collections.ObjectModel;
using System.ComponentModel;
using HabitManager.App.Services;
using HabitManager.Domain;

namespace HabitManager.App.ViewModels;

public class HabitListViewModel : INotifyPropertyChanged
{
    private readonly HabitService _habitService;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<Habit> Habits { get; private set; } = [];

    public HabitListViewModel(HabitService habitService)
    {
        _habitService = habitService;
    }

    public async Task LoadAsync()
    {
        await _habitService.InitializeAsync();

        Refresh();
    }

    public void Refresh()
    {
        ObservableCollection<Habit> visibleHabits = [];

        foreach (var habit in _habitService.Habits)
        {
            if (habit is { UserId: "demo-user-1", Status: HabitStatus.Active })
            {
                visibleHabits.Add(habit);
            }
        }

        Habits = visibleHabits;

        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(Habits)));
    }

    public async Task AddHabitAsync(Habit habit)
    {
        await _habitService.AddAsync(habit);

        Refresh();
    }
}