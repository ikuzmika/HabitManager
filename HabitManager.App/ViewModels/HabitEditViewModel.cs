using System.ComponentModel;
using HabitManager.Domain;

namespace HabitManager.App.ViewModels;

public class HabitEditViewModel : INotifyPropertyChanged
{
    private readonly Habit _habit;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Name { get; set; }

    public string? Description { get; set; }

    public string ErrorMessage { get; private set; } = string.Empty;

    public HabitEditViewModel(Habit habit)
    {
        _habit = habit;

        Name = habit.Name;
        Description = habit.Description;
    }

    public bool Save()
    {
        try
        {
            _habit.Update(Name, Description);

            ErrorMessage = string.Empty;

            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(nameof(ErrorMessage)));

            return true;
        }
        catch (ArgumentException exception)
        {
            ErrorMessage = exception.Message;

            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(nameof(ErrorMessage)));

            return false;
        }
    }
}