namespace HabitManager.DataModels;

public class HabitDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsArchived { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    
    public HabitScheduleDto Schedule { get; set; } = new();
    public List<HabitCompletionDto> Completions { get; set; } = [];
}