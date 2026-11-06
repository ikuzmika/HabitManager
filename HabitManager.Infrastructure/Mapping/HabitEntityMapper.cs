using HabitManager.Domain;
using HabitManager.Infrastructure.Entities;

namespace HabitManager.Infrastructure.Mapping;

public static class HabitEntityMapper
{
    public static Habit ToDomain(HabitEntity entity)
    {
        HabitSchedule schedule =
            entity.ScheduleType == HabitScheduleType.Daily
                ? new HabitSchedule()
                : new HabitSchedule(
                    entity.ScheduleDays
                        .Select(day => day.Day)
                        .ToList());

        List<HabitCompletion> completions = entity.Completions
            .Select(completion => HabitCompletion.Restore(
                completion.Id,
                completion.HabitId,
                completion.Date))
            .ToList();

        return Habit.Restore(
            entity.Id,
            entity.UserId,
            entity.Name,
            entity.Description,
            entity.Status,
            entity.CreatedAt,
            entity.UpdatedAt,
            schedule,
            completions);
    }

    public static HabitEntity ToEntity(Habit habit)
    {
        return new HabitEntity
        {
            Id = habit.Id,
            UserId = habit.UserId,
            Name = habit.Name,
            Description = habit.Description,
            Status = habit.Status,
            CreatedAt = habit.CreatedAt,
            UpdatedAt = habit.UpdatedAt,
            ScheduleType = habit.Schedule.Type,

            ScheduleDays = habit.Schedule.Days
                .Select(day => new HabitScheduleDayEntity
                {
                    HabitId = habit.Id,
                    Day = day
                })
                .ToList(),

            Completions = habit.Completions
                .Select(completion => new HabitCompletionEntity
                {
                    Id = completion.Id,
                    HabitId = habit.Id,
                    Date = completion.Date
                })
                .ToList()
        };
    }
}