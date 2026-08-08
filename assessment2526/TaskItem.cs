using SQLite;

namespace assessment2526;
/// <summary>
///  This class represents a task item in the task management application. It includes properties for the task's title, description, priority, due date and time, completion status, and location information.
///  The class also includes attributes for SQLite database mapping and computed properties for UI display purposes.
/// </summary>

public class TaskItem
{
    //primary key for the database, auto-incremented
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    //core task information
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    //priority level of the task, used for sorting and  indicators in the UI e.g.haptic and flashlight alerts
    public string Priority { get; set; } = "Medium";
    // date and time when the task is due, used for scheduling notifications and displaying in the UI
    public DateTime DueDateTime { get; set; }
    // indicates whether the task has been completed, used for filtering and displaying in the UI
    public bool IsCompleted { get; set; }
    // indicates whether a due date alert has been triggered, used to prevent multiple notifications for the same task
    public bool DueAlertTriggered { get; set; }
    // path to the image associated with the task, used for displaying in the UI
    public string ImagePath { get; set; } = string.Empty;
    // Store/location-related fields for geolocation-based alerts
    public string StoreAddress { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    

    // Determines whether an image should be displayed in the UI.
    [Ignore]
    public bool HasImage => !string.IsNullOrWhiteSpace(ImagePath);
    //  determine the color associated with the task's priority level for UI display
    [Ignore]
    public string PriorityColor
    {
        get
        {
            return Priority switch
            {
                "High" => "#D32F2F",
                "Medium" => "#F57C00",
                "Low" => "#388E3C",
                _ => "#616161"
            };
        }
    }
    // Provides a user-friendly status text based on the completion status of the task for UI display.
    [Ignore]
    public string StatusText => IsCompleted ? "Completed" : "Pending";
    //Hide the Complete button for completed tasks
    [Ignore]
    public bool ShowCompleteButton => !IsCompleted;
    // Adjusts the opacity of the task card in the UI to visually indicate completed tasks.
    [Ignore]
    public double CardOpacity => IsCompleted ? 0.72 : 1.0;
    // Changes the background color of the task card in the UI based on whether the task is completed, providing a visual cue to differentiate completed tasks from pending ones.
    [Ignore]
    public string CardBackground =>
        IsCompleted ? "#F2F2F2" : "#FFFFFF";
    // Adjusts the background color for dark mode based on the completion status of the task, providing a consistent visual cue in both light and dark themes.
    [Ignore]
    public string DarkCardBackground =>
        IsCompleted ? "#1A1A1A" : "#1E1E1E";
    // Applies a strikethrough text decoration to the task title in the UI if the task is completed.
    [Ignore]
    public TextDecorations TitleDecoration =>
    IsCompleted ? TextDecorations.Strikethrough : TextDecorations.None;
    // Determines whether location information is available for the task, which can be used to enable or disable location-based alerts and display location details in the UI.
    [Ignore]
    public bool HasLocation => !string.IsNullOrWhiteSpace(StoreAddress);

}