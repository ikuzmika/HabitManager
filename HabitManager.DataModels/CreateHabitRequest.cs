namespace HabitManager.DataModels;

public class CreateHabitRequest
{
    public string UserId { get; set; } =  string.Empty; // Pagaidu variants, vēlāk Id tiek noteikts pēc lietotāja
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    public HabitScheduleDto Schedule { get; set; } = new();
}