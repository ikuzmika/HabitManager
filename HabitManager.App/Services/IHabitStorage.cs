using HabitManager.Domain;

namespace HabitManager.App.Services;

public interface IHabitStorage
{
    Task<List<Habit>?> LoadAsync();
    
    Task SaveAsync(IReadOnlyList<Habit> habits);
}