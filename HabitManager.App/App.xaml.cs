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

    private static readonly HttpClient ApiHttpClient = new()
    {
    #if ANDROID && DEBUG
        BaseAddress = new Uri("http://10.0.2.2:5295/"),
    #else
        BaseAddress = new Uri("https://localhost:7248/"),
    #endif
        Timeout = TimeSpan.FromSeconds(15)
    };

    public static HabitService HabitService { get; } =
        new HabitService(new HabitApiClient(ApiHttpClient));
}