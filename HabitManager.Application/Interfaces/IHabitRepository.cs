using HabitManager.Domain;

namespace HabitManager.Application.Interfaces;

// Interfeiss nosaka, ka aplikācijas slānis iegūt un saglabā ieradumus
public interface IHabitRepository
{
    Task<IReadOnlyList<Habit>> GetAllAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Habit?> GetByIdAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Habit habit,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Habit habit,
        CancellationToken cancellationToken = default);
}