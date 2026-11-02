namespace CalculatorApp.Views;

public partial class SettingsPage : ContentPage
{
    private const string ThemePreferenceKey = "SelectedThemeSetting";
    private bool _isInitializing = true;

    public SettingsPage()
    {
        InitializeComponent();
        LoadCurrentThemeSetting();
    }

    // استرجاع الثيم المخزن وتحديد زر الراديو المناسب عند فتح الصفحة
    private void LoadCurrentThemeSetting()
    {
        _isInitializing = true;
        string savedTheme = Preferences.Get(ThemePreferenceKey, "System");

        switch (savedTheme)
        {
            case "Light":
                LightRadio.IsChecked = true;
                break;
            case "Dark":
                DarkRadio.IsChecked = true;
                break;
            default:
                SystemRadio.IsChecked = true;
                break;
        }
        _isInitializing = false;
    }

    // معالجة حدث تغيير زر الراديو وتطبيق الثيم فوراً
    private void OnThemeRadioButtonCheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (_isInitializing || !e.Value) return;

        if (sender == LightRadio)
        {
            ApplyAndSaveTheme("Light", AppTheme.Light);
        }
        else if (sender == DarkRadio)
        {
            ApplyAndSaveTheme("Dark", AppTheme.Dark);
        }
        else if (sender == SystemRadio)
        {
            ApplyAndSaveTheme("System", AppTheme.Unspecified);
        }
    }

    // حفظ الخيار في Preferences وتغيير ثيم التطبيق فورياً
    private void ApplyAndSaveTheme(string themeKey, AppTheme appTheme)
    {
        Preferences.Set(ThemePreferenceKey, themeKey);

        if (Application.Current != null)
        {
            Application.Current.UserAppTheme = appTheme;
        }
    }
}