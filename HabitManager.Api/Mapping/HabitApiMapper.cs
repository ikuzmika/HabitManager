using HabitManager.DataModels;
using HabitManager.Domain;

namespace HabitManager.Api.Mapping;

public static class HabitApiMapper
{
    public static HabitDto ToDto(Habit habit)
    {
        return new HabitDto
        {
            Id = habit.Id,
            UserId = habit.UserId,
            Name = habit.Name,
            Description = habit.Description,
            IsArchived = habit.Status == HabitStatus.Archived,
            CreatedAt = habit.CreatedAt,
            UpdatedAt = habit.UpdatedAt,

            Schedule = new HabitScheduleDto
            {
                IsDaily = habit.Schedule.Type == HabitScheduleType.Daily,
                Days = habit.Schedule.Days.ToList()
            },

            Completions = habit.Completions
                .Select(ToDto)
                .ToList()
        };
    }

    public static HabitCompletionDto ToDto(HabitCompletion completion)
    {
        return new HabitCompletionDto
        {
            Id = completion.Id,
            HabitId = completion.HabitId,
            Date = completion.Date
        };
    }

    public static HabitSchedule ToDomain(HabitScheduleDto? schedule)
    {
        if (schedule is null)
        {
            throw new ArgumentException("Habit schedule is required.");
        }

        if (schedule.IsDaily)
        {
            return new HabitSchedule();
        }

        if (schedule.Days is null || schedule.Days.Count == 0)
        {
            throw new ArgumentException("Please select at least one day.");
        }

        return new HabitSchedule(schedule.Days);
    }
}