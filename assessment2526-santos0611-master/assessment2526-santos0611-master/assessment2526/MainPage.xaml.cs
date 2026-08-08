using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Plugin.Maui.Audio;
using System.Linq;
using System.Windows.Input;

namespace assessment2526;
// Main task dashboard page.
// Handles task display, filtering, sorting, editing, completion,
// hardware triggered alerts, and gesture-based interactions.

public partial class MainPage : ContentPage
{
    private readonly TaskDatabase _database;
    private readonly IAudioManager _audioManager;
    private string _selectedFilter = "All"; // Stores the current filter/sort choices selected by the user
    private string _selectedSort = "Default";
    private bool _isCheckingDueTasks = false; // Used to prevent overlapping timer checks running at the same time
    private bool _checkingLocation = false;

    // Stores the last time a nearby-location reminder was shown for each task.
    // This allows the app to repeat the reminder every 30 seconds while the user stays nearby,
    private readonly Dictionary<int, DateTime> _lastLocationAlertTimes = new();

    // Long press command is bound from XAML using TouchBehavior
    public ICommand LongPressEditCommand { get; }

    public MainPage()
    {
        InitializeComponent();

        _database = Application.Current?.Handler?.MauiContext?.Services.GetService<TaskDatabase>()
                    ?? throw new InvalidOperationException("TaskDatabase service not found."); // Get the TaskDatabase service to access task data.

        _audioManager = Application.Current?.Handler?.MauiContext?.Services.GetService<IAudioManager>()
                    ?? throw new InvalidOperationException("AudioManager service not found."); // Get the AudioManager service for playing sounds.

        LongPressEditCommand = new Command<TaskItem>(async task => await OpenEditAsync(task)); // Allows long press on a task card to open the edit page

        FilterPicker.SelectedIndex = 0;
        SortPicker.SelectedIndex = 0;

        // Periodically checks for tasks that are now due
        Dispatcher.StartTimer(TimeSpan.FromSeconds(15), () =>
        {
            _ = CheckDueTasksAsync();
            return true;
        });

        // Periodically checks whether the user is near a saved store location
        // Runs every 30 seconds so nearby reminders can repeat while the user stays in range.
        Dispatcher.StartTimer(TimeSpan.FromSeconds(20), () =>
        {
            _ = CheckNearbyTasksAsync();
            return true;
        });
    }

    // Checks for tasks that have reached their due time.
    // Uses toast feedback, optional sound/haptics, and flashlight alerts.
    // The DueAlertTriggered flag prevents repeated alerts for the same task.
    private async Task CheckDueTasksAsync()
    {
        if (!Preferences.Get("due_task_enabled", true))
            return;

        if (_isCheckingDueTasks)
            return;

        _isCheckingDueTasks = true;

        try
        {
            var dueTasks = await _database.GetDueTasksAsync(DateTime.Now);

            if (dueTasks == null || dueTasks.Count == 0)
                return;

            // Show one grouped toast only once
            await Toast.Make(
                dueTasks.Count == 1
                    ? $"⏰ Due now: {dueTasks[0].Title}"
                    : $"⏰ {dueTasks.Count} tasks are now due",
                ToastDuration.Long,
                14
            ).Show();

            // Optional vibration / haptic feedback once
            if (Preferences.Get("vibration_enabled", true))
            {
                HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
            }

            foreach (var task in dueTasks)
            {
                // Flashlight alert based on priority
                await FlashForPriorityAsync(task.Priority);

                // Optional sound feedback
                if (Preferences.Get("sound_enabled", true))
                {
                    string soundFile = task.Priority switch
                    {
                        "High" => "high.mp3",
                        "Medium" => "medium.mp3",
                        "Low" => "low.mp3",
                        _ => "medium.mp3"
                    };

                    using var stream = await FileSystem.OpenAppPackageFileAsync(soundFile);
                    var player = _audioManager.CreatePlayer(stream);
                    player.Play();
                }

                task.DueAlertTriggered = true;
                await _database.SaveTaskAsync(task);
            }

            await LoadTasksAsync();
        }
        finally
        {
            _isCheckingDueTasks = false;
        }
    }
    // Uses the device flashlight as a due task alert.
    // Flash count changes depending on the task priority.
    private async Task FlashForPriorityAsync(string priority)
    {
        if (!Preferences.Get("due_task_enabled", true))
            return;

        bool supported = await Flashlight.Default.IsSupportedAsync();

        if (!supported)
            return;

        int flashCount = priority switch
        {
            "High" => 3,
            "Medium" => 2,
            "Low" => 1,
            _ => 1
        };

        for (int i = 0; i < flashCount; i++)
        {
            try
            {
                await Flashlight.Default.TurnOnAsync();
                await Task.Delay(300);

                await Flashlight.Default.TurnOffAsync();
                await Task.Delay(250);
            }
            catch
            {
                break; // If flashlight is in use by another app or an error occurs, stop trying to flash.
            }
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadTasksAsync(); // Refresh task list whenever the page becomes visible again
        await CheckNearbyTasksAsync(); // Check for nearby tasks immediately when returning to the page, in case user has moved since last check.
        await ShowSwipeHintIfNeededAsync(); // Shows a one-time hint about swipe actions if the user hasn't seen it before.
    }

    // Loads tasks from the database using the currently selected filter and sort options.
    private async Task LoadTasksAsync()
    {
        var tasks = await _database.GetFilteredAndSortedTasksAsync(_selectedFilter, _selectedSort);

        TasksCollectionView.ItemsSource = tasks;
        EmptyStateLayout.IsVisible = tasks == null || tasks.Count == 0;
        TotalTasksLabel.Text = tasks.Count.ToString();
        PendingTasksLabel.Text = tasks.Count(t => !t.IsCompleted).ToString();
        CompletedTasksLabel.Text = tasks.Count(t => t.IsCompleted).ToString();
   }

    // Updates the selected filter and sort state when either picker changes.
    private async void OnSelectionChanged(object sender, EventArgs e)
    {
        if (FilterPicker.SelectedItem is string selectedFilter)
            _selectedFilter = selectedFilter;

        if (SortPicker.SelectedItem is string selectedSort)
            _selectedSort = selectedSort;

        await LoadTasksAsync();
    }

    // Reads the task aloud using text-to-speech.
    // Supports accessibility and provides a hands-free way to review task details.
    private async void OnSpeakClicked(object sender, EventArgs e)
    {
        if (!Preferences.Get("speech_enabled", true))
            return;

        // Gets the task associated with the clicked speak button and constructs a descriptive string to read aloud.
        if (sender is Button button && button.BindingContext is TaskItem task)
        {
            string textToSpeak =
                $"Task title: {task.Title}. " +
                $"Description: {task.Description}. " +
                $"Priority: {task.Priority}. " +
                $"Due date: {task.DueDateTime:dd MMMM yyyy, HH:mm}. " +
                $"Status: {task.StatusText}.";

            await TextToSpeech.Default.SpeakAsync(textToSpeak);
        }
    }

    // Marks a task as completed from the standard button and gives feedback based on priority.
    private async void OnCompleteClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is TaskItem task)
        {
            if (task.IsCompleted)
                return;

            task.IsCompleted = true;
            await _database.SaveTaskAsync(task);

            if (Preferences.Get("vibration_enabled", true))
            {
                switch (task.Priority) // haptic feedback based on priority first, then show alert with message
                {
                    case "High":
                        HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
                        break;

                    case "Medium":
                    case "Low":
                        HapticFeedback.Default.Perform(HapticFeedbackType.Click);
                        break;
                }
            }

            // Select sound file by priority
            string soundFile = task.Priority switch
            {
                "High" => "high.mp3",
                "Medium" => "medium.mp3",
                "Low" => "low.mp3",
                _ => "medium.mp3"
            };

            if (Preferences.Get("sound_enabled", true))
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync(soundFile);
                var player = _audioManager.CreatePlayer(stream);
                player.Play();
            }

            string message = task.Priority switch
            {
                "High" => "🔥 Big win! You completed an important task!",
                "Medium" => "👏 Nice work! Task completed.",
                "Low" => "✅ Good job! Keep going!",
                _ => "Task completed!"
            };

            await DisplayAlert("🎉 Congratulations!", message, "OK");
            await LoadTasksAsync();
        }
    }

    // Deletes a task from the standard delete button after user confirmation.
    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is TaskItem task)
        {
            bool confirm = await DisplayAlert("Delete Task",
                                              $"Delete '{task.Title}'?",
                                              "Yes",
                                              "No");

            if (!confirm)
                return;

            await _database.DeleteTaskAsync(task);
            await LoadTasksAsync();
        }
    }

    // Opens the selected task in edit mode from the edit button
    private async void OnEditClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is TaskItem task)
        {
            await Navigation.PushAsync(new NewTask(task));
        }
    }

    // Opens the edit mode when the user long presses the task
    private async Task OpenEditAsync(TaskItem? task)
    {
        if (task == null)
            return;

        await Navigation.PushAsync(new NewTask(task));
    }

    // Opens task creation page from floating add button
    private async void OnAddTaskClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NewTask());
    }

    // Deletes the task when the user swipes left on the task card and confirms deletion in the alert
    private async void OnSwipeDeleteInvoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem && swipeItem.BindingContext is TaskItem task)
        {
            bool confirm = await DisplayAlert("Delete Task",
                                              $"Delete '{task.Title}'?",
                                              "Yes",
                                              "No");

            if (!confirm)
                return;

            await _database.DeleteTaskAsync(task);
            await LoadTasksAsync();
        }
    }

    // Marks the task as completed when the user swipes right on the task card and gives feedback based on priority
    // Uses the same completion logic pattern as the normal button.
    private async void OnSwipeCompletedInvoked(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem && swipeItem.BindingContext is TaskItem task)
        {
            if (task.IsCompleted)
                return;

            task.IsCompleted = true;
            await _database.SaveTaskAsync(task);

            if (Preferences.Get("vibration_enabled", true))
            {
                switch (task.Priority)
                {
                    case "High":
                        HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
                        break;

                    case "Medium":
                    case "Low":
                        HapticFeedback.Default.Perform(HapticFeedbackType.Click);
                        break;
                }
            }

            string soundFile = task.Priority switch
            {
                "High" => "high.mp3",
                "Medium" => "medium.mp3",
                "Low" => "low.mp3",
                _ => "medium.mp3"
            };

            if (Preferences.Get("sound_enabled", true))
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync(soundFile);
                var player = _audioManager.CreatePlayer(stream);
                player.Play(); // Play a sound based on the priority of the completed task
            }

            string message = task.Priority switch
            {
                "High" => "🔥 Big win! You completed an important task!",
                "Medium" => "👏 Nice work! Task completed.",
                "Low" => "✅ Good job! Keep going!",
                _ => "Task completed!"
            };

            await DisplayAlert("🎉 Congratulations!", message, "OK");
            await LoadTasksAsync();
        }
    }

    // Checks whether the user is physically near any saved task location.
    // If within the radius, a toast popup is shown every 30 seconds while the user remains nearby.
    private async Task CheckNearbyTasksAsync()
    {
        if (!Preferences.Get("location_enabled", true))
            return;

        if (_checkingLocation)
            return;

        _checkingLocation = true;

        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
                return;

            var location = await Geolocation.Default.GetLocationAsync();

            if (location == null)
                return;

            var tasks = await _database.GetTasksAsync();

            foreach (var task in tasks)
            {
                // Skip tasks with no stored location
                if (task.Latitude == 0 && task.Longitude == 0)
                    continue;

                double distanceInKm = Location.CalculateDistance(
                    location.Latitude,
                    location.Longitude,
                    task.Latitude,
                    task.Longitude,
                    DistanceUnits.Kilometers);

                double distanceInMeters = distanceInKm * 1000;

                // Task is considered nearby if within 250 metres of the saved location
                if (distanceInMeters <= 250)
                {
                    // Only allow a new popup every 30 seconds for the same task
                    if (_lastLocationAlertTimes.TryGetValue(task.Id, out DateTime lastShown))
                    {
                        if ((DateTime.Now - lastShown).TotalSeconds < 30)
                            continue;
                    }

                    if (Preferences.Get("sound_enabled", true))
                    {
                        using var stream = await FileSystem.OpenAppPackageFileAsync("location.mp3");
                        var player = _audioManager.CreatePlayer(stream);
                        player.Play(); // Play a sound to alert the user that they're near a task location
                    }

                    if (Preferences.Get("vibration_enabled", true))
                    {
                        HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
                    }

                    await Toast.Make(
                        $"📍 You're near: {task.Title}",
                        ToastDuration.Long
                    ).Show();

                    _lastLocationAlertTimes[task.Id] = DateTime.Now;
                }
            }
        }
        finally
        {
            _checkingLocation = false;
        }
    }

    // Shows a one time onboarding hint to teach the user about swipe gestures.
    private async Task ShowSwipeHintIfNeededAsync()
    {
        bool alreadyShown = Preferences.Get("swipe_hint_shown", false);

        if (alreadyShown)
            return;

        var toast = Toast.Make(
            "Tip: Swipe left to delete and swipe right to complete tasks.",
            ToastDuration.Long,
            14);

        await toast.Show();

        Preferences.Set("swipe_hint_shown", true);
    }
}