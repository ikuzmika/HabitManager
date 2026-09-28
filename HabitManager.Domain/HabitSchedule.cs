namespace HabitManager.Domain;

/*
 * Kods tika veidots konsultējoties ar MI (ChatGPT)
 */

public class HabitSchedule
{
    private readonly List<DayOfWeek> _days = [];
    
    public HabitScheduleType Type { get; }
    public IReadOnlyList<DayOfWeek> Days => _days.AsReadOnly();

    // Konstruktori
    
    public HabitSchedule()
    {
        Type = HabitScheduleType.Daily;
    }
    
    public HabitSchedule(List<DayOfWeek> days) // Ieraduma atkārtošanās nodrošināšana
    {
        if (days == null || days.Count == 0)
        {
            throw new ArgumentException("Please select at least one day");
        }

        foreach (DayOfWeek day in days)
        {
            if (!Enum.IsDefined(typeof(DayOfWeek), day))
            {
                throw new ArgumentException("Selected day is not a valid");
            }

            if (!_days.Contains(day))
            {
                _days.Add(day);
            }
        }
        
        Type = HabitScheduleType.SelectedDays;
    }
    
    /*
     *  Metodes
     */

    public bool IsScheduledOn(DateOnly date) // Ieraduma atkārtojamības pārbaude
    {
        return Type == HabitScheduleType.Daily || _days.Contains(date.DayOfWeek);
    }
}