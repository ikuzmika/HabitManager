namespace HabitManager.Domain;

public class HabitCompletion
{
    public Guid Id { get; }
    public Guid HabitId { get; }
    public DateOnly Date { get; }
    
    // Jauna izpildes ieraksta izveide
    internal HabitCompletion(Guid habitId, DateOnly date)
        : this(Guid.NewGuid(), habitId, date)
    {
    }
    
    // Inicializācija jauniem un atjaunotiem ierakstiem
    private HabitCompletion(Guid id, Guid habitId, DateOnly date)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                "Completion ID must not be empty.", nameof(id));
        }

        if (habitId == Guid.Empty)
        {
            throw new ArgumentException(
                "Habit ID must not be empty.", nameof(habitId));
        }

        Id = id;
        HabitId = habitId;
        Date = date;
    }
    
    // Izpildes ieraksta atjaunošana no saglabātiem datiem 
    public static HabitCompletion Restore(
        Guid id,
        Guid habitId,
        DateOnly date)
    {
        return new HabitCompletion(id, habitId, date);
    }
}