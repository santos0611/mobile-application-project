using SQLite;

namespace assessment2526;
/// <summary>
/// handels all database operations for the task management application, including creating the database
/// It uses SQLite for data storage.
/// </summary>

public class TaskDatabase
{
    private readonly SQLiteAsyncConnection _database;

    public TaskDatabase(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);
        // Ensure the TaskItem table is created when the database is started.
        // This will create the table if it doesn't exist, and do nothing if it already exists.
        _database.CreateTableAsync<TaskItem>().Wait();
    }
    // returns all tasks from the database as a list of TaskItem objects. It uses asynchronous operations to avoid blocking the UI thread.
    public Task<List<TaskItem>> GetTasksAsync()
    {
        return _database.Table<TaskItem>().ToListAsync();
    }
    //returns the task thats Id matches the provided id. It uses asynchronous operations to avoid blocking the UI thread.
    // Used by the timer-based due-task checking logic on the main page.
    public Task<List<TaskItem>> GetDueTasksAsync(DateTime now)
    {
        return _database.Table<TaskItem>()
            .Where(t => !t.IsCompleted && !t.DueAlertTriggered && t.DueDateTime <= now)
            .ToListAsync();
    }
    // Returns tasks after applying the selected filter and sort option.
    // This supports the user-facing filter/sort controls on the home page

    public async Task<List<TaskItem>> GetFilteredAndSortedTasksAsync(string filter, string sort)
    {
        var query = _database.Table<TaskItem>();
        // Apply filtering based on the selected filter option. This allows users to view tasks based on their completion status or priority.

        query = filter switch
        {
            "Completed" => query.Where(t => t.IsCompleted),
            "Pending" => query.Where(t => !t.IsCompleted),
            "High Priority" => query.Where(t => t.Priority == "High"),
            "Medium Priority" => query.Where(t => t.Priority == "Medium"),
            "Low Priority" => query.Where(t => t.Priority == "Low"),
            _ => query
        };
        // Apply sorting based on the selected sort option.
        // This allows users to order tasks by due date or title in either ascending or descending order.

        query = sort switch
        {
            "Due Date Asc" => query.OrderBy(t => t.DueDateTime),
            "Due Date Desc" => query.OrderByDescending(t => t.DueDateTime),
            "Title A-Z" => query.OrderBy(t => t.Title),
            "Title Z-A" => query.OrderByDescending(t => t.Title),
            _ => query
        };

        return await query.ToListAsync();
    }
    //Inserts a new task if it has no ID, or updates an existing task if it already exists.
    // This method is reused for both task creation and task editing.

    public Task<int> SaveTaskAsync(TaskItem task)
    {
        if (task.Id != 0)
            return _database.UpdateAsync(task);

        return _database.InsertAsync(task);
    }
    // Deletes the specified task from the database. This is used when a user chooses to delete a task from the home page.
    public Task<int> DeleteTaskAsync(TaskItem task)
    {
        return _database.DeleteAsync(task);
    }
}