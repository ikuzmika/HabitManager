namespace HabitManager.DataModels;

public class HabitCompletionRequest
{
    public Guid HabitId { get; set; }
    public DateOnly Date { get; set; }
}