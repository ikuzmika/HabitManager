using System.ComponentModel;
using HabitManager.Domain;

namespace HabitManager.App.ViewModels;

public class HabitCreateViewModel : INotifyPropertyChanged
{
    private readonly string _userId;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string ErrorMessage { get; private set; } = string.Empty;

    public HabitCreateViewModel(string userId)
    {
        _userId = userId;
    }

    public Habit? Create()
    {
        try
        {
            Habit habit = new Habit(_userId, Name, Description);

            ErrorMessage = string.Empty;

            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(nameof(ErrorMessage)));

            return habit;
        }
        catch (ArgumentException exception)
        {
            ErrorMessage = exception.Message;

            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(nameof(ErrorMessage)));

            return null;
        }
    }
}