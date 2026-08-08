using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace assessment2526;
// ViewModel for creating and editing tasks.
// Handles form state, validation, and saving task data to SQLite.

public class NewTaskViewModel : INotifyPropertyChanged
{
    private readonly TaskDatabase _database;
    // Backing fields for task properties

    private string _title = string.Empty;
    private string _description = string.Empty;
    private string _priority = "Medium";
    private DateTime _dueDate = DateTime.Today;
    private TimeSpan _dueTime = DateTime.Now.TimeOfDay;
    private string _statusMessage = string.Empty;
    private string _imagePath = string.Empty;
    private double _latitude;
    private double _longitude;
    private string _storeAddress = string.Empty;
    private int _taskId; // Used to track whether we're editing an existing task or creating a new one

    public event PropertyChangedEventHandler? PropertyChanged;
    // title entered by the user, required field fortask creation
    public event EventHandler? TaskSaved;
    // Event triggered after a task is successfully saved, used to notify the view to show a confirmation and navigate back
    public string Title
    {
        get => _title;
        set { _title = value; OnPropertyChanged(); }
    }
    // optional description entered by the user for additional task details
    public string Description
    {
        get => _description;
        set { _description = value; OnPropertyChanged(); }
    }
    // priority level selected by the user, used for sorting and visual indicators in the UI and device alerts
    public string Priority
    {
        get => _priority;
        set { _priority = value; OnPropertyChanged(); }
    }
    // Date portion of the due time
    public DateTime DueDate
    {
        get => _dueDate;
        set
        {
            _dueDate = value;
            OnPropertyChanged();
        }
    }
    // Time portion of the due time
    public TimeSpan DueTime
    {
        get => _dueTime;
        set
        {
            _dueTime = value;
            OnPropertyChanged();
        }
    }
    // Coordinates used for location based task alerts
    public double Latitude
    {
        get => _latitude;
        set { _latitude = value; OnPropertyChanged(); }
    }

    public double Longitude
    {
        get => _longitude;
        set { _longitude = value; OnPropertyChanged(); }
    }

    // Status message to provide user feedback on task saving operations, displayed in the UI
    public string StatusMessage
    {
        get => _statusMessage;
        set { _statusMessage = value; OnPropertyChanged(); }
    }
    // local image path for a photo attached to the task
    public string ImagePath
    {
        get => _imagePath;
        set { _imagePath = value; OnPropertyChanged(); }
    }
    // Store/address text entered or resolved through geocoding
    public string StoreAddress
    {
        get => _storeAddress;
        set
        {
            _storeAddress = value;
            OnPropertyChanged();
        }
    }
    // Button text changes depending on whether user is saving a new task or updating an existing task
    public string SaveButtonText => TaskId == 0 ? "Save Task" : "Update Task";
    public int TaskId
    {
        get => _taskId;
        set
        {
            _taskId = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SaveButtonText));
        }
    }

    public ICommand SaveCommand { get; }
    public ICommand ClearCommand { get; }

    public NewTaskViewModel()
    {
        _database = Application.Current?.Handler?.MauiContext?.Services.GetService<TaskDatabase>()
                    ?? throw new InvalidOperationException("TaskDatabase service not found.");

        // Commands are bound to the UI buttons in XAML
        SaveCommand = new Command(async () => await SaveTaskAsync());
        ClearCommand = new Command(ClearForm);
    }
    // Overloaded constructor used when editing an existing task.
    // Prefills the form with the selected task's existing information for editing.
    public NewTaskViewModel(TaskItem task) : this()
    {
        TaskId = task.Id;
        Title = task.Title;
        Description = task.Description;
        Priority = task.Priority;
        DueDate = task.DueDateTime.Date;
        DueTime = task.DueDateTime.TimeOfDay;
        ImagePath = task.ImagePath;
        StoreAddress = task.StoreAddress;
        Latitude = task.Latitude;
        Longitude = task.Longitude;
    }

    // Validates user input and either inserts a new task or updates an existing one.
    private async Task SaveTaskAsync()
    {
        //basic validation to ensure required fields are filled out before saving to the database. 
        if (string.IsNullOrWhiteSpace(Title))
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            StatusMessage = "Task title is required.";
            _ = ClearStatusMessageAfterDelayAsync();
            return;
        }
        if (Title.Trim().Length > 50)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            StatusMessage = "Task title must be 50 characters or fewer."; // Enforces a character limit on the title to prevent UI issues and ensure concise task names.
            _ = ClearStatusMessageAfterDelayAsync();
            return;
        }
        DateTime selectedDueDateTime = DueDate.Date + DueTime;

        if (selectedDueDateTime < DateTime.Now)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            StatusMessage = "Choose a due date and time that is not in the past.";
            _ = ClearStatusMessageAfterDelayAsync();
            return;
        }// Ensures that the due date and time selected by the user is in the future, preventing tasks from being created with past due dates which would not make sense for a task list application.

        try
        {
            var task = new TaskItem
            {
                Id = TaskId,
                Title = Title.Trim(),
                Description = Description?.Trim() ?? string.Empty,
                Priority = Priority,
                DueDateTime = DueDate.Date + DueTime,// Combines separate date and time inputs into one DateTime value
                IsCompleted = false,// Currently defaults edited tasks back to not completed 
                ImagePath = ImagePath,
                StoreAddress = StoreAddress,
                Latitude = Latitude,
                Longitude = Longitude
            };

            await _database.SaveTaskAsync(task);
            // Triggers an event to notify the view that the task was saved, allowing it to show a confirmation and navigate back to the task list.
            TaskSaved?.Invoke(this, EventArgs.Empty);
            // feedback to the user that the task was saved successfully
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
            ClearForm();
            StatusMessage = string.Empty;
        }
        catch
        {
            // Prevents app crashing and provides user feedback if save fails
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            StatusMessage = "Task could not be saved. Please try again.";
            _ = ClearStatusMessageAfterDelayAsync();
        }
    }
    // Resets the form back to its default state after save or when clear is pressed.
    private void ClearForm()
    {
        TaskId = 0;
        Title = string.Empty;
        Description = string.Empty;
        Priority = "Medium";
        DueDate = DateTime.Today;
        DueTime = DateTime.Now.TimeOfDay;
        ImagePath = string.Empty;
        StoreAddress = string.Empty;
        Latitude = 0;
        Longitude = 0;
    }
    // Clears temporary bottom status messages after 5 seconds.
    private async Task ClearStatusMessageAfterDelayAsync()
    {
        await Task.Delay(5000);
        StatusMessage = string.Empty;
    }
    // Standard MVVM property change notification so the UI updates automatically.
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}