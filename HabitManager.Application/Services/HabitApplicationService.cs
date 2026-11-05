using HabitManager.Application.Interfaces;
using HabitManager.Domain;

namespace HabitManager.Application.Services;

public class HabitApplicationService
{
    private readonly IHabitRepository _repository;

    public HabitApplicationService(IHabitRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<Habit>> GetAllAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return _repository.GetAllAsync(userId, cancellationToken);
    }

    public async Task<Habit> GetByIdAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        Habit? habit = await _repository.GetByIdAsync(
            id, userId, cancellationToken);

        return habit ?? throw new KeyNotFoundException("Habit was not found.");
    }

    public async Task<Habit> CreateAsync(
        string userId,
        string name,
        string? description,
        HabitSchedule schedule,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(schedule);

        Habit habit = new Habit(userId, name, description);
        habit.ChangeSchedule(schedule);

        await _repository.AddAsync(habit, cancellationToken);

        return habit;
    }

    public async Task<Habit> UpdateAsync(
        Guid id,
        string userId,
        string name,
        string? description,
        HabitSchedule schedule,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(schedule);

        Habit habit = await GetByIdAsync(
            id, userId, cancellationToken);

        if (habit.Status == HabitStatus.Archived)
        {
            throw new InvalidOperationException(
                "An archived habit cannot be edited.");
        }

        habit.Update(name, description);
        habit.ChangeSchedule(schedule);

        await _repository.UpdateAsync(habit, cancellationToken);

        return habit;
    }

    public async Task SetArchivedAsync(
        Guid id,
        string userId,
        bool isArchived,
        CancellationToken cancellationToken = default)
    {
        Habit habit = await GetByIdAsync(
            id, userId, cancellationToken);

        if (isArchived)
        {
            habit.Archive();
        }
        else
        {
            habit.Unarchive();
        }

        await _repository.UpdateAsync(habit, cancellationToken);
    }

    public async Task CompleteAsync(
        Guid id,
        string userId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        Habit habit = await GetByIdAsync(
            id, userId, cancellationToken);

        habit.Complete(date);

        await _repository.UpdateAsync(habit, cancellationToken);
    }

    public async Task UndoCompletionAsync(
        Guid id,
        string userId,
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        Habit habit = await GetByIdAsync(
            id, userId, cancellationToken);

        habit.UndoCompletion(date);

        await _repository.UpdateAsync(habit, cancellationToken);
    }

    public async Task<IReadOnlyList<HabitCompletion>> GetCompletionsAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        Habit habit = await GetByIdAsync(
            id, userId, cancellationToken);

        return habit.Completions;
    }
}