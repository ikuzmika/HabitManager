namespace HabitManager.App.Controls;

public class ThemeToolbarItem : ToolbarItem
{
    public ThemeToolbarItem()
    {
        // Text = "Change theme";
        Order = ToolbarItemOrder.Primary;
        Priority = 0;

        
        this.SetAppTheme<ImageSource>(
            IconImageSourceProperty,
            ImageSource.FromFile("bulb_on.png"),
            ImageSource.FromFile("bulb_off.png"));

        Clicked += OnThemeClicked;
    }

    private void OnThemeClicked(object? sender, EventArgs e)
    {
        Application? application = Application.Current;

        if (application == null)
        {
            return;
        }

        application.UserAppTheme =
            application.RequestedTheme == AppTheme.Dark
                ? AppTheme.Light
                : AppTheme.Dark;

        Preferences.Default.Set(
            "AppTheme",
            application.UserAppTheme.ToString());
    }
}