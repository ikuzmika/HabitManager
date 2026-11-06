using System.ComponentModel;
using System.Windows.Input;
using HabitManager.Domain;
using HabitManager.App.Services;

namespace HabitManager.App.ViewModels;

public class HabitDetailsViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public Habit SelectedHabit { get; private set; }

    public ICommand CompleteTodayCommand { get; }
    
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

        await ApplyAndSaveAsync(async () =>
        {
            if (SelectedHabit.IsCompletedOn(today))
            {
                await _habitService.UndoCompletionAsync(
                    SelectedHabit.Id, today);
            }
            else
            {
                await _habitService.CompleteAsync(
                    SelectedHabit.Id, today);
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
            new PropertyChangedEventArgs(nameof(CompletedToday)));

        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(StatusText)));
        
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(CompletionButtonText)));
        
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(CanEdit)));

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