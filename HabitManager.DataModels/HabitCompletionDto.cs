namespace HabitManager.DataModels;

public class HabitCompletionDto
{
    public Guid Id { get; set; }
    public Guid HabitId { get; set; }
    public DateOnly Date { get; set; }
}