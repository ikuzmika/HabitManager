using HabitManager.Domain;

namespace HabitManager.App.ViewModels;

public class HabitListItemViewModel
{
    public Habit Habit { get; }

    public string Name => Habit.Name;

    public string? Description => Habit.Description;

    public string CompletionText
    {
        get
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            if (Habit.IsCompletedOn(today))
            {
                return "Completed today";
            }

            return "Not completed today";
        }
    }

    public HabitListItemViewModel(Habit habit)
    {
        Habit = habit;
    }
}