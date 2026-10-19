using Microsoft.Extensions.DependencyInjection;
using HabitManager.App.Services;

namespace HabitManager.App;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
    
    public static HabitService HabitService { get; } =
        new HabitService(new JsonHabitStorage());
}