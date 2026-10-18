using HabitManager.Domain;
using HabitManager.TestData;

/*
 * Kods tika veidots konsultējoties ar MI (ChatGPT)
 */

DateOnly today = DateOnly.FromDateTime(DateTime.Today);
List<Habit> habits = TestDataFactory.Create(today);

Console.WriteLine("Habit Manager");
Console.WriteLine($"Today: {today:dd.MM.yyyy}");
Console.WriteLine($"Total number of habits: {habits.Count}");

List<string> userIds = [];

foreach (Habit habit in habits)
{
    if (!userIds.Contains(habit.UserId))
    {
        userIds.Add(habit.UserId);
    }
}

Console.WriteLine($"Users: {string.Join(", ", userIds)}");

foreach (string userId in userIds)
{
    Console.WriteLine($"\nUser {userId}");

    foreach (Habit habit in habits)
    {
        if (habit.UserId != userId)
        {
            continue;
        }

        string statusText = habit.Status == HabitStatus.Active ? "Active" : "Archived";

        string completedText = habit.IsCompletedOn(today) ? "Yes" : "No";

        Console.WriteLine($"Habit: {habit.Name}");
        Console.WriteLine($"Description: {habit.Description}");
        Console.WriteLine($"Status: {statusText}");
        Console.WriteLine($"Completed today?: {completedText}");

        Console.WriteLine(habit.Schedule.Type == HabitScheduleType.Daily
            ? "Schedule: Daily"
            : $"Schedule: {string.Join(", ", habit.Schedule.Days)}");

        Console.WriteLine($"Number of completed records: {habit.Completions.Count}");

        foreach (HabitCompletion completion in habit.Completions)
        {
            Console.WriteLine($" - {completion.Date:dd.MM.yyyy}");
        }

        Console.WriteLine();
    }
}

Console.WriteLine("\nExample of completing habit:");

foreach (Habit habit in habits)
{
    if (habit.Status == HabitStatus.Active && !habit.IsCompletedOn(today))
    {
        Console.WriteLine($"Habit: {habit.Name}");
        Console.WriteLine($"Is it complete: {habit.IsCompletedOn(today)}");
        habit.Complete(today);
        Console.WriteLine($"Is it complete now: {habit.IsCompletedOn(today)}");

        break;
    }
}