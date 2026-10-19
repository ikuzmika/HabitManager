using System.ComponentModel;
using System.Windows.Input;
using HabitManager.Domain;
using HabitManager.App.Services;

namespace HabitManager.App.ViewModels;

public class HabitDetailsViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public Habit SelectedHabit { get; }

    public ICommand CompleteTodayCommand { get; }
    
    public ICommand ArchiveCommand { get; }

    public string StatusText => SelectedHabit.Status == HabitStatus.Active ? "Active" : "Archived";
    
    private readonly HabitService _habitService;
    
    private bool _isSaving;
    
    public string ErrorMessage { get; private set; } = "";

    public string CompletedToday
    {
        get
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            return SelectedHabit.IsCompletedOn(today) ? "Yes" : "No";
        }
    }

    public HabitDetailsViewModel(
        Habit habit,
        HabitService habitService)
    {
        SelectedHabit = habit;
        _habitService = habitService;

        CompleteTodayCommand = new Command(
            async () => await CompleteTodayAsync(),
            CanCompleteToday);

        ArchiveCommand = new Command(
            async () => await ArchiveHabitAsync(),
            CanArchive);
    }
    

    private bool CanCompleteToday()
    {
        return !_isSaving &&
               SelectedHabit.Status == HabitStatus.Active;
    }

    private bool CanArchive()
    {
        return !_isSaving &&
               SelectedHabit.Status == HabitStatus.Active;
    }

    private async Task CompleteTodayAsync()
    {
        if (!CanCompleteToday())
        {
            return;
        }

        DateOnly today = DateOnly.FromDateTime(DateTime.Today);

        await ApplyAndSaveAsync(() =>
        {
            if (SelectedHabit.IsCompletedOn(today))
            {
                SelectedHabit.UndoCompletion(today);
            }
            else
            {
                SelectedHabit.Complete(today);
            }
        });
    }

    private async Task ArchiveHabitAsync()
    {
        if (!CanArchive())
        {
            return;
        }

        await ApplyAndSaveAsync(
            () => SelectedHabit.Archive());
    }
    
    private async Task ApplyAndSaveAsync(Action change)
    {
        _isSaving = true;
        ErrorMessage = "";

        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(ErrorMessage)));

        Refresh();

        try
        {
            change();

            await _habitService.SaveAsync();
        }
        catch (Exception exception)
        {
            ErrorMessage =
                "The operation could not be completed." +
                " If the changes are already visible on the screen," +
                " they may not yet have been saved to the file." +
                exception.Message;

            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(nameof(ErrorMessage)));
        }
        finally
        {
            _isSaving = false;
            Refresh();
        }
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
        
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(CompletionButtonText)));

        ((Command)CompleteTodayCommand).ChangeCanExecute();
        ((Command)ArchiveCommand).ChangeCanExecute();
    }
    
    public string CompletionButtonText
    {
        get
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            return SelectedHabit.IsCompletedOn(today) ? "Undo today's completion" : "Mark as completed today";
        }
    }
}