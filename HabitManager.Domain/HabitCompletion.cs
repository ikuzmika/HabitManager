namespace HabitManager.Domain;

public class HabitCompletion
{
    public Guid Id { get; }
    public Guid HabitId { get; }
    public DateOnly Date { get; }

    internal HabitCompletion(Guid habitId, DateOnly date)
    {
        Id = Guid.NewGuid();
        HabitId = habitId;
        Date = date;
    }
}