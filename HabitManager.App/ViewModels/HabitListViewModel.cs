using System.Collections.ObjectModel;
using System.ComponentModel;
using HabitManager.Domain;
using HabitManager.TestData;

namespace HabitManager.App.ViewModels;

public class HabitListViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<Habit> Habits { get; private set; } = [];

    public HabitListViewModel()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);

        List<Habit> habits = TestDataFactory.Create(today);

        foreach (var habit in habits)
        {
            if (habit is { UserId: "demo-user-1", Status: HabitStatus.Active })
            {
                Habits.Add(habit);
            }
        }
    }

    public void Refresh()
    {
        ObservableCollection<Habit> updatedHabits = [];

        foreach (var habit in Habits)
        {
            if (habit.Status == HabitStatus.Active)
            {
                updatedHabits.Add(habit);
            }
        }

        Habits = updatedHabits;

        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(Habits)));
    }
    
    public void AddHabit(Habit habit)
    {
        Habits.Add(habit);
    }
}