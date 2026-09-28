using HabitManager.Domain;
using HabitManager.DataModels;

Habit habit = new Habit(
    "demo-user-1",
    "Read for 20 minutes",
    "In the evening before bed");

Console.WriteLine($"Habit ID: {habit.Id}");
Console.WriteLine($"User: {habit.UserId}");
Console.WriteLine($"Name: {habit.Name}");
Console.WriteLine($"Description: {habit.Description}");
Console.WriteLine($"Status: {habit.Status}");
Console.WriteLine($"Created at: {habit.CreatedAt}");

Console.WriteLine("\nAfter update:");
habit.Update(
    "Read for 30 minutes",
    "After breakfast before work");
Console.WriteLine($"Name: {habit.Name}");
Console.WriteLine($"Description: {habit.Description}");
Console.WriteLine($"Updated at: {habit.UpdatedAt}");

Console.WriteLine("\nAfter archivation:");
habit.Archive();
Console.WriteLine($"Name: {habit.Name}");
Console.WriteLine($"Status: {habit.Status}");
Console.WriteLine($"Updated at: {habit.UpdatedAt}");

Console.WriteLine("\nCompletion test:");
Habit musicHabit = new Habit(
    "demo-user-1",
    "Listen to 3 new songs by EXO",
    "");
DateOnly today = DateOnly.FromDateTime(DateTime.Today);
Console.WriteLine($"Before: {musicHabit.IsCompletedOn(today)}");
musicHabit.Complete(today);
Console.WriteLine($"After: {musicHabit.IsCompletedOn(today)}");
Console.WriteLine($"Number of completed records: {musicHabit.Completions.Count} ");

Console.WriteLine("\nSchedule test:");
Habit walkingHabit = new Habit(
    "demo-user-2",
    "Walking",
    "Walking in nearby park");
DateOnly friday = new DateOnly(2026, 09, 25);
DateOnly wednesday = new DateOnly(2026, 09, 30);
Console.WriteLine($"Original schedule: {walkingHabit.Schedule.Type}");
Console.WriteLine(
    $"Is walking scheduled for wednesday: {walkingHabit.Schedule.IsScheduledOn(wednesday)}"); // Ieradums atkartojas katru dienu, tāpēc trešdienā ir atkartojums
HabitSchedule walkingSchedule = new HabitSchedule(
[
    DayOfWeek.Friday,
    DayOfWeek.Monday
]);
walkingHabit.ChangeSchedule(walkingSchedule);
Console.WriteLine($"New schedule: {walkingHabit.Schedule.Type}");
Console.WriteLine($"Is walking scheduled for friday: {walkingHabit.Schedule.IsScheduledOn(friday)}");
Console.WriteLine(
    $"Is walking scheduled for wednesday: {walkingHabit.Schedule.IsScheduledOn(wednesday)}"); // Grafikā ir tikai piektdiena un pirmdiena, trešdienā ieradums nav ieplanots


Console.WriteLine($"\nData models test:");
CreateHabitRequest request = new CreateHabitRequest
{
    UserId = "demo-user-3",
    Name = "Watch movie with family",
    Description = "The Batman movie",
};

Habit languageHabit = new Habit(
    request.UserId,
    request.Name,
    request.Description);

HabitSchedule requestSchedule =
    request.Schedule.IsDaily ? new HabitSchedule() : new HabitSchedule(request.Schedule.Days);

languageHabit.ChangeSchedule(requestSchedule);

HabitDto habitDto = new HabitDto
{
    Id = languageHabit.Id,
    UserId = languageHabit.UserId,
    Name = languageHabit.Name,
    Description = languageHabit.Description,
    IsArchived = languageHabit.Status == HabitStatus.Archived,
    CreatedAt = languageHabit.CreatedAt,
    UpdatedAt = languageHabit.UpdatedAt,

    Schedule = new HabitScheduleDto
    {
        IsDaily = languageHabit.Schedule.Type == HabitScheduleType.Daily,
        Days = new List<DayOfWeek>(languageHabit.Schedule.Days)
    }
};

Console.WriteLine($"Name: {habitDto.Name}");
Console.WriteLine($"User: {habitDto.UserId}");
Console.WriteLine($"Archived: {habitDto.IsArchived}");
Console.WriteLine($"Daily: {habitDto.Schedule.IsDaily}");