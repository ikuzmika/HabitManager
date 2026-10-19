using HabitManager.DataModels;
using HabitManager.Domain;

namespace HabitManager.App.Mapping;

// Sagatavo datus saglabāšanai un pēc ielādes atjauno domēna objektus
public static class HabitMapper
{
    public static HabitDto ToDto(Habit habit)
    {
        HabitDto dto = new HabitDto
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
                Days = new List<DayOfWeek>(habit.Schedule.Days)
            }
        };

        foreach (HabitCompletion completion in habit.Completions)
        {
            HabitCompletionDto completionDto = new HabitCompletionDto
            {
                Id = completion.Id,
                HabitId = completion.HabitId,
                Date = completion.Date
            };

            dto.Completions.Add(completionDto);
        }

        return dto;
    }

    public static Habit ToDomain(HabitDto dto)
    {
        HabitSchedule schedule = dto.Schedule.IsDaily ? new HabitSchedule() : new HabitSchedule(dto.Schedule.Days);

        HabitStatus status = dto.IsArchived ? HabitStatus.Archived : HabitStatus.Active;

        List<HabitCompletion> completions = [];

        foreach (var completionDto in dto.Completions)
        {
            HabitCompletion completion = HabitCompletion.Restore(
                completionDto.Id,
                completionDto.HabitId,
                completionDto.Date);

            completions.Add(completion);
        }

        return Habit.Restore(
            dto.Id,
            dto.UserId,
            dto.Name,
            dto.Description,
            status,
            dto.CreatedAt,
            dto.UpdatedAt,
            schedule,
            completions);
    }
}