using HabitManager.Domain;

namespace HabitManager.Infrastructure.Entities;

public class HabitEntity
{
    public Guid Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public HabitStatus Status { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public HabitScheduleType ScheduleType { get; set; }

    public List<HabitScheduleDayEntity> ScheduleDays { get; set; } = [];

    public List<HabitCompletionEntity> Completions { get; set; } = [];
}