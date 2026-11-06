namespace HabitManager.Infrastructure.Entities;

public class HabitCompletionEntity
{
    public Guid Id { get; set; }

    public Guid HabitId { get; set; }

    public DateOnly Date { get; set; }

    public HabitEntity Habit { get; set; } = null!;
}