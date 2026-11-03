using System.ComponentModel;
using HabitManager.Domain;

namespace HabitManager.App.ViewModels;

public class HabitScheduleViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private bool _isDaily = true;

    public bool IsDaily
    {
        get => _isDaily;
        set
        {
            if (_isDaily == value)
            {
                return;
            }

            _isDaily = value;

            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(nameof(IsDaily)));

            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(nameof(IsSelectedDays)));
        }
    }

    public bool IsSelectedDays => !IsDaily;

    public List<ScheduleDayViewModel> Days { get; } =
    [
        new(DayOfWeek.Monday),
        new(DayOfWeek.Tuesday),
        new(DayOfWeek.Wednesday),
        new(DayOfWeek.Thursday),
        new(DayOfWeek.Friday),
        new(DayOfWeek.Saturday),
        new(DayOfWeek.Sunday)
    ];

    public HabitScheduleViewModel(HabitSchedule? schedule = null)
    {
        if (schedule == null)
        {
            return;
        }

        IsDaily = schedule.Type == HabitScheduleType.Daily;

        foreach (ScheduleDayViewModel day in Days)
        {
            day.IsSelected = schedule.Days.Contains(day.Day);
        }
    }

    public HabitSchedule Build()
    {
        if (IsDaily)
        {
            return new HabitSchedule();
        }

        List<DayOfWeek> selectedDays = Days
            .Where(day => day.IsSelected)
            .Select(day => day.Day)
            .ToList();

        return new HabitSchedule(selectedDays);
    }

    public static string Describe(HabitSchedule schedule)
    {
        if (schedule.Type == HabitScheduleType.Daily)
        {
            return "Every day";
        }

        HabitScheduleViewModel viewModel = new(schedule);

        return string.Join(
            ", ",
            viewModel.Days
                .Where(day => day.IsSelected)
                .Select(day => day.Name));
    }
}

public class ScheduleDayViewModel
{
    public DayOfWeek Day { get; }

    public string Name => Day.ToString();

    public bool IsSelected { get; set; }

    public ScheduleDayViewModel(DayOfWeek day)
    {
        Day = day;
    }
}