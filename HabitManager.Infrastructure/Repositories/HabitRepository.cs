using HabitManager.Application.Interfaces;
using HabitManager.Domain;
using HabitManager.Infrastructure.Entities;
using HabitManager.Infrastructure.Mapping;
using HabitManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HabitManager.Infrastructure.Repositories;

public class HabitRepository : IHabitRepository
{
    private readonly HabitDbContext _context;

    public HabitRepository(HabitDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Habit>> GetAllAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var entities = await _context.Habits
            .AsNoTracking()
            .Include(habit => habit.ScheduleDays)
            .Include(habit => habit.Completions)
            .Where(habit => habit.UserId == userId)
            .OrderBy(habit => habit.CreatedAt)
            .ToListAsync(cancellationToken);

        return entities
            .Select(HabitEntityMapper.ToDomain)
            .ToList();
    }

    public async Task<Habit?> GetByIdAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Habits
            .AsNoTracking()
            .Include(habit => habit.ScheduleDays)
            .Include(habit => habit.Completions)
            .SingleOrDefaultAsync(
                habit => habit.Id == id && habit.UserId == userId,
                cancellationToken);

        return entity is null
            ? null
            : HabitEntityMapper.ToDomain(entity);
    }

    public async Task AddAsync(
        Habit habit,
        CancellationToken cancellationToken = default)
    {
        var entity = HabitEntityMapper.ToEntity(habit);

        _context.Habits.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        Habit habit,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Habits
            .Include(item => item.ScheduleDays)
            .Include(item => item.Completions)
            .SingleOrDefaultAsync(
                item => item.Id == habit.Id &&
                        item.UserId == habit.UserId,
                cancellationToken);

        if (entity is null)
        {
            throw new KeyNotFoundException("Habit was not found.");
        }

        entity.Name = habit.Name;
        entity.Description = habit.Description;
        entity.Status = habit.Status;
        entity.UpdatedAt = habit.UpdatedAt;
        entity.ScheduleType = habit.Schedule.Type;

        UpdateScheduleDays(entity, habit);
        UpdateCompletions(entity, habit);

        await _context.SaveChangesAsync(cancellationToken);
    }

    private void UpdateScheduleDays(HabitEntity entity, Habit habit)
    {
        var requestedDays = habit.Schedule.Days.ToHashSet();

        var removedDays = entity.ScheduleDays
            .Where(day => !requestedDays.Contains(day.Day))
            .ToList();

        foreach (var day in removedDays)
        {
            entity.ScheduleDays.Remove(day);
            _context.HabitScheduleDays.Remove(day);
        }

        var existingDays = entity.ScheduleDays
            .Select(day => day.Day)
            .ToHashSet();

        foreach (var day in requestedDays)
        {
            if (existingDays.Contains(day))
            {
                continue;
            }

            entity.ScheduleDays.Add(new HabitScheduleDayEntity
            {
                HabitId = entity.Id,
                Day = day
            });
        }
    }

    private void UpdateCompletions(HabitEntity entity, Habit habit)
    {
        var requestedIds = habit.Completions
            .Select(completion => completion.Id)
            .ToHashSet();

        var removedCompletions = entity.Completions
            .Where(completion => !requestedIds.Contains(completion.Id))
            .ToList();

        foreach (var completion in removedCompletions)
        {
            entity.Completions.Remove(completion);
            _context.HabitCompletions.Remove(completion);
        }

        var existingIds = entity.Completions
            .Select(completion => completion.Id)
            .ToHashSet();

        foreach (var completion in habit.Completions)
        {
            if (existingIds.Contains(completion.Id))
            {
                continue;
            }

            entity.Completions.Add(new HabitCompletionEntity
            {
                Id = completion.Id,
                HabitId = entity.Id,
                Date = completion.Date
            });
        }
    }
}