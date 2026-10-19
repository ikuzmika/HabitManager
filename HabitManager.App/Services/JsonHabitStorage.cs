using System.Text.Json;
using HabitManager.App.Mapping;
using HabitManager.DataModels;
using HabitManager.Domain;
using Microsoft.Maui.Storage;

namespace HabitManager.App.Services;

// Nolasa un ieraksta JSON failu
public class JsonHabitStorage : IHabitStorage
{
    private readonly string _filePath;

    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true
    };

    public JsonHabitStorage()
    {
        _filePath = Path.Combine(
            FileSystem.AppDataDirectory,
            "habits.json");
    }

    public async Task<List<Habit>?> LoadAsync()
    {
        if (!File.Exists(_filePath))
        {
            return null;
        }

        string json = await File.ReadAllTextAsync(_filePath);

        List<HabitDto>? dtos =
            JsonSerializer.Deserialize<List<HabitDto>>(json, _options);

        if (dtos == null)
        {
            throw new InvalidDataException(
                "The habits file does not contain a valid list.");
        }

        List<Habit> habits = [];

        foreach (var dto in dtos)
        {
            Habit habit = HabitMapper.ToDomain(dto);
            habits.Add(habit);
        }

        return habits;
    }

    public async Task SaveAsync(IReadOnlyList<Habit> habits)
    {
        List<HabitDto> dtos = [];

        foreach (var habit in habits)
        {
            HabitDto dto = HabitMapper.ToDto(habit);
            dtos.Add(dto);
        }

        string json = JsonSerializer.Serialize(dtos, _options);

        string temporaryPath = _filePath + ".tmp";

        await File.WriteAllTextAsync(temporaryPath, json);

        File.Move(temporaryPath, _filePath, overwrite: true);
    }
}