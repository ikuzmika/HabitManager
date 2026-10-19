using System.ComponentModel;
using System.Windows.Input;
using HabitManager.Domain;

namespace HabitManager.App.ViewModels;

public class HabitDetailsViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public Habit SelectedHabit { get; }

    public ICommand CompleteTodayCommand { get; }
    
    public ICommand ArchiveCommand { get; }

    public string StatusText => SelectedHabit.Status == HabitStatus.Active ? "Active" : "Archived";

    public string CompletedToday
    {
        get
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            return SelectedHabit.IsCompletedOn(today) ? "Yes" : "No";
        }
    }

    public HabitDetailsViewModel(Habit habit)
    {
        SelectedHabit = habit;

        CompleteTodayCommand = new Command(
            CompleteToday,
            CanCompleteToday);

        ArchiveCommand = new Command(
            ArchiveHabit,
            CanArchive);
    }
    

    private bool CanCompleteToday()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);

        return SelectedHabit.Status == HabitStatus.Active
               && !SelectedHabit.IsCompletedOn(today);
    }

    private void CompleteToday()
    {
        if (!CanCompleteToday())
        {
            return;
        }

        DateOnly today = DateOnly.FromDateTime(DateTime.Today);

        SelectedHabit.Complete(today);

        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(CompletedToday)));

        ((Command)CompleteTodayCommand).ChangeCanExecute();
    }
    
    private bool CanArchive()
    {
        return SelectedHabit.Status == HabitStatus.Active;
    }

    private void ArchiveHabit()
    {
        if (!CanArchive())
        {
            return;
        }

        SelectedHabit.Archive();

        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(StatusText)));

        ((Command)ArchiveCommand).ChangeCanExecute();
        ((Command)CompleteTodayCommand).ChangeCanExecute();
    }
    
    public void Refresh()
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(SelectedHabit)));

        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(CompletedToday)));

        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(StatusText)));

        ((Command)CompleteTodayCommand).ChangeCanExecute();
        ((Command)ArchiveCommand).ChangeCanExecute();
    }
}