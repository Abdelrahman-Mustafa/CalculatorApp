namespace CalculatorApp;

public partial class App : Application
{
    private const string ThemePreferenceKey = "SelectedThemeSetting";

    public App()
    {
        InitializeComponent();
        ApplyStoredTheme();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }

    private void ApplyStoredTheme()
    {
        string savedTheme = Preferences.Get(ThemePreferenceKey, "System");

        UserAppTheme = savedTheme switch
        {
            "Light" => AppTheme.Light,
            "Dark" => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };
    }
}