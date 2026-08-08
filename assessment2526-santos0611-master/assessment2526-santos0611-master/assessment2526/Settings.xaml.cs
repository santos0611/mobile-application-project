namespace assessment2526;

public partial class Settings : ContentPage
{
    public Settings()
    {
        InitializeComponent();
        LoadSettings();
    }

    private void LoadSettings()// Loads saved settings from Preferences and updates the UI controls to reflect those values when the settings page is opened.
    {
        SoundSwitch.IsToggled = Preferences.Get("sound_enabled", true);
        VibrationSwitch.IsToggled = Preferences.Get("vibration_enabled", true);
        SpeechSwitch.IsToggled = Preferences.Get("speech_enabled", true);
        LocationSwitch.IsToggled = Preferences.Get("location_enabled", true);
        DueTaskSwitch.IsToggled = Preferences.Get("due_task_enabled", true);
        LargeTextSwitch.IsToggled = Preferences.Get("large_text_enabled", false);

        string savedTheme = Preferences.Get("theme_mode", "System");
        ThemePicker.SelectedItem = savedTheme;
    }

    private async void OnSaveSettingsClicked(object sender, EventArgs e)// Saves the current settings to Preferences when the user clicks the save button. It also applies changes immediately for theme and text size, and shows a confirmation alert.
    {
        Preferences.Set("sound_enabled", SoundSwitch.IsToggled);
        Preferences.Set("vibration_enabled", VibrationSwitch.IsToggled);
        Preferences.Set("speech_enabled", SpeechSwitch.IsToggled);
        Preferences.Set("location_enabled", LocationSwitch.IsToggled);
        Preferences.Set("due_task_enabled", DueTaskSwitch.IsToggled);
        Preferences.Set("large_text_enabled", LargeTextSwitch.IsToggled);

        string selectedTheme = ThemePicker.SelectedItem?.ToString() ?? "System";
        Preferences.Set("theme_mode", selectedTheme);

        // Applies light / dark theme immediately.
        Application.Current!.UserAppTheme = selectedTheme switch
        {
            "Light" => AppTheme.Light,
            "Dark" => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };

        // Applies larger or normal font sizes across the app immediately.
        bool largeText = LargeTextSwitch.IsToggled;

        if (Application.Current?.Resources != null)
        {
            Application.Current.Resources["TitleFontSize"] = largeText ? 30d : 24d;
            Application.Current.Resources["SectionFontSize"] = largeText ? 22d : 18d;
            Application.Current.Resources["BodyFontSize"] = largeText ? 18d : 14d;
            Application.Current.Resources["SmallFontSize"] = largeText ? 16d : 13d;
            Application.Current.Resources["ButtonFontSize"] = largeText ? 18d : 14d;
        }

        StatusLabel.Text = "Settings saved successfully.";

        await DisplayAlert("Settings", "Your preferences have been saved.", "OK");
    }
}