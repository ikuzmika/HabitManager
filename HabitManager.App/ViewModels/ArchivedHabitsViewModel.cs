using System.Collections.ObjectModel;
using HabitManager.App.Services;
using HabitManager.Domain;

namespace HabitManager.App.ViewModels;

public class ArchivedHabitsViewModel
{
    private readonly HabitService _habitService;

    public ObservableCollection<Habit> Habits { get; } = [];

    public ArchivedHabitsViewModel(HabitService habitService)
    {
        _habitService = habitService;
    }

    public void Refresh()
    {
        Habits.Clear();

        foreach (var habit in _habitService.Habits)
        {
            if (habit is { UserId: "demo-user-1", Status: HabitStatus.Archived })
            {
                Habits.Add(habit);
            }
        }
    }

    public async Task UnarchiveAsync(Habit habit)
    {
        if (habit.UserId != "demo-user-1")
        {
            throw new InvalidOperationException(
                "This habit belongs to another user.");
        }

        await _habitService.SetArchivedAsync(habit.Id, false);

        Refresh();
    }
    
    public async Task LoadAsync()
    {
        await _habitService.InitializeAsync();
        Refresh();
    }
}