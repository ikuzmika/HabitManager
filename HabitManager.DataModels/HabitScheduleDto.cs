namespace HabitManager.DataModels;

public class HabitScheduleDto // Objekts datu pārsūtīšanai
{
    public bool IsDaily { get; set; } =  true;
    public List<DayOfWeek> Days { get; set; } = [];
}