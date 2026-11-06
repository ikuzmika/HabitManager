using System.ComponentModel;
using HabitManager.App.Services;
using HabitManager.Domain;

namespace HabitManager.App.ViewModels;

public class HabitEditViewModel : INotifyPropertyChanged
{
    private readonly Habit _habit;
    private readonly HabitService _habitService;

    public event PropertyChangedEventHandler? PropertyChanged;

    public HabitScheduleViewModel Schedule { get; }
    public string Name { get; set; }

    public string? Description { get; set; }

    public string ErrorMessage { get; private set; } = "";

    public HabitEditViewModel(
        Habit habit,
        HabitService habitService)
    {
        _habit = habit;
        _habitService = habitService;

        Schedule = new HabitScheduleViewModel(habit.Schedule);
        Name = habit.Name;
        Description = habit.Description;
    }

    public async Task<bool> SaveAsync()
    {
        try
        {
            HabitSchedule schedule = Schedule.Build();

            await _habitService.UpdateAsync(
                _habit.Id,
                Name,
                Description,
                schedule);

            SetError("");
            return true;
        }
        catch (Exception exception)
        {
            SetError(exception.Message);
            return false;
        }
    }

    private void SetError(string message)
    {
        ErrorMessage = message;

        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(ErrorMessage)));
    }
}