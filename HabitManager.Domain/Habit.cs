namespace HabitManager.Domain;

public class Habit
{
    private readonly List<HabitCompletion> _completions = [];

    // private set, lai vērtību varētu mainīt tikai šīs klases kods
    // Guid tiek izmantots priekš ieraduma unikālas identificēšanas 
    public Guid Id { get; private set; }
    public string UserId { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public HabitStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    
    public HabitSchedule Schedule { get; private set; } = new HabitSchedule();
    public IReadOnlyList<HabitCompletion> Completions => _completions.AsReadOnly();
    
    // Konstruktors
    public Habit (string userId, string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(userId))
        { 
            throw new ArgumentException(
                "The habit must be linked to the user.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "The habit name must not be left blank.");
        }
        
        string trimmedName = name.Trim();

        if (trimmedName.Length > 200)
        {
            throw new ArgumentException(
                "The habit name must not be longer than 200 characters.");
        }
        
        Id = Guid.NewGuid();
        UserId = userId;
        Name = trimmedName;
        Description = description;
        Status = HabitStatus.Active;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = CreatedAt;
    }
    
    /*
     * Metodes
     */
    
    public void Update(string name, string? description) // Metode priekš ieraduma labošanas
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "The habit name must not be left blank.");
        }
        
        string trimmedName = name.Trim();

        if (trimmedName.Length > 200)
        {
            throw new ArgumentException(
                "The habit name must not be longer than 200 characters.");
        }
        
        Name = trimmedName;
        Description = description;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
    
    public void Archive() // Metode priekš ieraduma arhivēšanas
    {
        if (Status == HabitStatus.Archived)
        {
            return;
        }
        
        Status = HabitStatus.Archived;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Complete(DateOnly date)
    {
        if (Status == HabitStatus.Archived)
        {
            throw new InvalidOperationException(
                "The habit is already archived.");
        }

        if (IsCompletedOn(date))
        {
            throw new InvalidOperationException(
                "On this date the habit has already been completed.");
        }

        HabitCompletion completion = new HabitCompletion(Id, date);
        
        _completions.Add(completion);
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public bool IsCompletedOn(DateOnly date) // Metode, lai pārbaudītu ieraduma izpildes datumu
    {
        foreach (HabitCompletion completion in _completions)
        {
            if (completion.Date == date)
            {
                return true;
            }
        }
        
        return false;
    }

    public void ChangeSchedule(HabitSchedule schedule) // Metode grafika maiņai
    {
        Schedule = schedule ?? throw new ArgumentNullException(nameof(schedule));
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}