namespace HabitManager.DataModels;

public class UpdateHabitRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    public HabitScheduleDto Schedule { get; set; } = new();
}