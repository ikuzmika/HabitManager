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

            _habit.Update(Name, Description);
            _habit.ChangeSchedule(schedule);
        }
        catch (ArgumentException exception)
        {
            SetError(exception.Message);
            return false;
        }

        try
        {
            await _habitService.SaveAsync();
        }
        catch (Exception exception)
        {
            SetError(
                "Changes have been made in the app’s memory, " +
                "but they could not be saved to a file. Please try saving again. " +
                exception.Message);

            return false;
        }

        SetError("");
        return true;
    }

    private void SetError(string message)
    {
        ErrorMessage = message;

        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(ErrorMessage)));
    }
}