namespace HabitManager.Infrastructure.Entities;

public class HabitScheduleDayEntity
{
    public Guid HabitId { get; set; }

    public DayOfWeek Day { get; set; }

    public HabitEntity Habit { get; set; } = null!;
}