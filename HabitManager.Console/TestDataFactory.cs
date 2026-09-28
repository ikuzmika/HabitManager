using HabitManager.Domain;
namespace HabitManager.Console;

/*
 * Kods tika veidots konsultējoties ar MI
 */

public static class TestDataFactory
{
    public static List<Habit> Create(DateOnly today)
    {
        string firstUserId = "demo-user-1";
        string seconduserId = "demo-user-2";

        Habit reading = new Habit(
            firstUserId,
            "Read for 20 minutes",
            "In the evening before bed");

        reading.Complete(today.AddDays(-2));
        reading.Complete(today.AddDays(-1));
        reading.Complete(today);
        
        Habit walking = new Habit(
            firstUserId,
            "Walking",
            "Walking in nearby park");
        
        walking.Complete(today.AddDays(-1));

        Habit language = new Habit(
            seconduserId,
            "Learning korean",
            "Memorising at leat 4 new words");
        
        language.Complete(today.AddDays(-3));
        language.Archive();
        
        Habit listening = new Habit(
            seconduserId,
            "Listen to 3 new songs by EXO",
            "");
        
        listening.Complete(today.AddDays(-1));

        Habit movie = new Habit(
            seconduserId,
            "Watch movie with family",
            "One of Marvel movie");
        
        movie.ChangeSchedule(
            new HabitSchedule(
            [
                DayOfWeek.Monday,
                DayOfWeek.Friday
            ]));
        
        return [reading, walking, language, listening, movie];
    }
}