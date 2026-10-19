using HabitManager.Domain;
using HabitManager.TestData;

namespace HabitManager.App.Services;

// Glabā ieradumus atmiņā, sagatavo datus, izsauc saglabāšanu 
public class HabitService
{
    private readonly IHabitStorage _storage;

    private readonly List<Habit> _habits = [];

    private bool _isInitialized;

    public IReadOnlyList<Habit> Habits => _habits.AsReadOnly();

    public HabitService(IHabitStorage storage)
    {
        _storage = storage;
    }

    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        List<Habit>? loadedHabits = await _storage.LoadAsync();

        if (loadedHabits == null)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            loadedHabits = TestDataFactory.Create(today);

            await _storage.SaveAsync(loadedHabits);
        }

        _habits.Clear();
        _habits.AddRange(loadedHabits);

        _isInitialized = true;
    }

    public async Task SaveAsync()
    {
        EnsureInitialized();

        await _storage.SaveAsync(_habits);
    }

    public async Task AddAsync(Habit habit)
    {
        EnsureInitialized();

        if (_habits.Any(existingHabit => existingHabit.Id == habit.Id))
        {
            throw new InvalidOperationException(
                "A habit with this ID already exists.");
        }

        _habits.Add(habit);

        try
        {
            await SaveAsync();
        }
        catch
        {
            _habits.Remove(habit);
            throw;
        }
    }

    private void EnsureInitialized()
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException(
                "Habit data has not been loaded yet.");
        }
    }
}