using HabitManager.App.Mapping;
using HabitManager.DataModels;
using HabitManager.Domain;

namespace HabitManager.App.Services;

public class HabitService
{
    private readonly HabitApiClient _apiClient;
    private readonly List<Habit> _habits = [];

    public IReadOnlyList<Habit> Habits => _habits.AsReadOnly();

    public HabitService(HabitApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task InitializeAsync()
    {
        var habitDtos = await _apiClient.GetAllAsync();

        // Vispirms pārveidojam visus saņemtos datus.
        var loadedHabits = habitDtos
            .Select(HabitMapper.ToDomain)
            .ToList();

        _habits.Clear();
        _habits.AddRange(loadedHabits);
    }

    public async Task AddAsync(Habit habit)
    {
        var dto = HabitMapper.ToDto(habit);

        var request = new CreateHabitRequest
        {
            UserId = habit.UserId,
            Name = habit.Name,
            Description = habit.Description,
            Schedule = dto.Schedule
        };

        var createdDto = await _apiClient.CreateAsync(request);

        var createdHabit = HabitMapper.ToDomain(createdDto);

        _habits.Add(createdHabit);
    }
    
    public async Task UpdateAsync(
        Guid id,
        string name,
        string? description,
        HabitSchedule schedule)
    {
        var request = new UpdateHabitRequest
        {
            Id = id,
            Name = name,
            Description = description,
            Schedule = new HabitScheduleDto
            {
                IsDaily = schedule.Type == HabitScheduleType.Daily,
                Days = schedule.Days.ToList()
            }
        };

        var updatedDto = await _apiClient.UpdateAsync(request);
        var updatedHabit = HabitMapper.ToDomain(updatedDto);

        int index = _habits.FindIndex(habit => habit.Id == id);

        if (index >= 0)
        {
            _habits[index] = updatedHabit;
        }
        else
        {
            _habits.Add(updatedHabit);
        }
    }
    
    public async Task CompleteAsync(Guid id, DateOnly date)
    {
        await _apiClient.CompleteAsync(id, date);
        await ReloadAfterChangeAsync();
    }

    public async Task UndoCompletionAsync(Guid id, DateOnly date)
    {
        await _apiClient.UndoCompletionAsync(id, date);
        await ReloadAfterChangeAsync();
    }

    public async Task SetArchivedAsync(Guid id, bool isArchived)
    {
        await _apiClient.SetArchivedAsync(id, isArchived);
        await ReloadAfterChangeAsync();
    }

    private async Task ReloadAfterChangeAsync()
    {
        try
        {
            await InitializeAsync();
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException(
                "The change was saved, but the updated data could not be loaded. " +
                "Return to the habit list to reload it. " +
                exception.Message,
                exception);
        }
    }
}