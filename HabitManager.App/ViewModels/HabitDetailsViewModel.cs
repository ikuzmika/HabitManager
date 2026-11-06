using System.ComponentModel;
using System.Windows.Input;
using HabitManager.Domain;
using HabitManager.App.Services;

namespace HabitManager.App.ViewModels;

public class HabitDetailsViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public Habit SelectedHabit { get; private set; }

    public ICommand CompleteSelectedDateCommand { get; }
    
    public ICommand ArchiveCommand { get; }

    public string StatusText => SelectedHabit.Status == HabitStatus.Active ? "Active" : "Archived";
    
    private readonly HabitService _habitService;
    
    private bool _isSaving;
    
    public bool CanEdit =>
        !_isSaving &&
        SelectedHabit.Status == HabitStatus.Active;
    
    public string ScheduleText =>
        HabitScheduleViewModel.Describe(SelectedHabit.Schedule);
    
    public string ErrorMessage { get; private set; } = "";
    
    private DateTime? _selectedDate = DateTime.Today;

    public string CompletedOnSelectedDate
    {
        get
        {
            if (SelectedDate is not DateTime selectedDate)
            {
                return "Select a date";
            }

            DateOnly date = DateOnly.FromDateTime(selectedDate);

            return SelectedHabit.IsCompletedOn(date) ? "Yes" : "No";
        }
    }

    public HabitDetailsViewModel(
        Habit habit,
        HabitService habitService)
    {
        SelectedHabit = habit;
        _habitService = habitService;

        CompleteSelectedDateCommand = new Command(
            async () => await CompleteSelectedDateAsync(),
            CanCompleteSelectedDate);

        ArchiveCommand = new Command(
            async () => await ArchiveHabitAsync(),
            CanArchive);
    }
    

    private bool CanCompleteSelectedDate()
    {
        return !_isSaving &&
               SelectedDate.HasValue &&
               SelectedHabit.Status == HabitStatus.Active;
    }

    private bool CanArchive()
    {
        return !_isSaving &&
               SelectedHabit.Status == HabitStatus.Active;
    }

    private async Task CompleteSelectedDateAsync()
    {
        if (!CanCompleteSelectedDate() ||
            SelectedDate is not DateTime selectedDate)
        {
            return;
        }

        DateOnly date = DateOnly.FromDateTime(selectedDate);

        await ApplyAndSaveAsync(async () =>
        {
            if (SelectedHabit.IsCompletedOn(date))
            {
                await _habitService.UndoCompletionAsync(
                    SelectedHabit.Id, date);
            }
            else
            {
                await _habitService.CompleteAsync(
                    SelectedHabit.Id, date);
            }
        });
    }

    private async Task ArchiveHabitAsync()
    {
        if (!CanArchive())
        {
            return;
        }

        await ApplyAndSaveAsync(() =>
            _habitService.SetArchivedAsync(SelectedHabit.Id, true));
    }
    
    private async Task ApplyAndSaveAsync(Func<Task> change)
    {
        if (_isSaving)
        {
            return;
        }

        _isSaving = true;
        ErrorMessage = "";

        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(ErrorMessage)));

        Refresh();

        try
        {
            await change();
        }
        catch (Exception exception)
        {
            ErrorMessage = exception.Message;

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
        var updatedHabit = _habitService.Habits
            .FirstOrDefault(habit => habit.Id == SelectedHabit.Id);

        if (updatedHabit is not null)
        {
            SelectedHabit = updatedHabit;
        }
        
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(ScheduleText)));
        
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(SelectedHabit)));

        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(CompletedOnSelectedDate)));

        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(StatusText)));
        
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(CompletionButtonText)));
        
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(CanEdit)));

        ((Command)CompleteSelectedDateCommand).ChangeCanExecute();
        ((Command)ArchiveCommand).ChangeCanExecute();
    }
    
    public string CompletionButtonText
    {
        get
        {
            if (SelectedDate is not DateTime selectedDate)
            {
                return "Select a date";
            }

            DateOnly date = DateOnly.FromDateTime(selectedDate);

            return SelectedHabit.IsCompletedOn(date)
                ? "Undo completion for selected date"
                : "Mark as completed for selected date";
        }
    }
    
    public DateTime? SelectedDate
    {
        get => _selectedDate;
        set
        {
            if (_selectedDate == value)
            {
                return;
            }

            _selectedDate = value;

            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(nameof(SelectedDate)));

            ErrorMessage = "";

            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(nameof(ErrorMessage)));

            Refresh();
        }
    }
}