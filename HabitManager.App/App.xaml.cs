using Microsoft.Extensions.DependencyInjection;
using HabitManager.App.Services;

namespace HabitManager.App;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        
        string savedTheme = Preferences.Default.Get(
            "AppTheme",
            "System");

        UserAppTheme = savedTheme switch
        {
            "Dark" => AppTheme.Dark,
            "Light" => AppTheme.Light,
            _ => AppTheme.Unspecified
        };
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
    
    public static HabitService HabitService { get; } =
        new HabitService(new JsonHabitStorage());
    
}