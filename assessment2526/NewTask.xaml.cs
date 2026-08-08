using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.ComponentModel;

namespace assessment2526;
// Code for the task creation/edit page.
// Handles hardware/API actions that are not suitable to place directly in the ViewModel,
// such as camera capture and geocoding.

public partial class NewTask : ContentPage
{
    private readonly NewTaskViewModel _viewModel;
    // Constructor used when creating a brand new task.
    public NewTask()
    {
        InitializeComponent();
        _viewModel = new NewTaskViewModel();
        _viewModel.TaskSaved += OnTaskSaved;
        BindingContext = _viewModel;
    }
    // Constructor used when editing an existing task.
    // Preloads the form with the selected task's existing values.
    public NewTask(TaskItem task)
    {
        InitializeComponent();
        _viewModel = new NewTaskViewModel(task);
        _viewModel.TaskSaved += OnTaskSaved;
        BindingContext = _viewModel;
    }
    // Event handler for when the task is saved in the ViewModel.
    // Shows a confirmation toast and navigates back to the previous page.
    private async void OnTaskSaved(object? sender, EventArgs e)
    {
        await Toast.Make(
            "✅ Task saved successfully",
            ToastDuration.Short
        ).Show();
        await Navigation.PopAsync();
    }

    private async void OnAttachPhotoClicked(object sender, EventArgs e)
    {
        try
        {
            // Prevents camera code running on unsupported devices/emulators
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                await DisplayAlert("Camera", "Camera capture is not supported on this device.", "OK");
                return;
            }

            var photo = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
            {
                Title = "Take task photo"
            });

            // User may cancel the camera without taking a photo
            if (photo == null)
                return;

            // Photos are copied into app storage so they remain available after capture
            var imagesFolder = Path.Combine(FileSystem.AppDataDirectory, "task_images");
            Directory.CreateDirectory(imagesFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
            var localFilePath = Path.Combine(imagesFolder, fileName);

            using var sourceStream = await photo.OpenReadAsync();
            using var localFileStream = File.OpenWrite(localFilePath);
            await sourceStream.CopyToAsync(localFileStream);

            _viewModel.ImagePath = localFilePath;
            _viewModel.StatusMessage = "Photo attached.";
            // Clear the bottom status message after 5 seconds
            _ = ClearStatusMessageAfterDelayAsync();
        }
        catch (Exception ex)
        {
            // Prevents crash and shows a readable error message
            await DisplayAlert("Camera Error", ex.Message, "OK");
        }
    }

    // Converts the entered store/address text into latitude and longitude using geocoding.
    // This allows location-based reminders to work without requiring the user to be physically
    // present at the store when creating the task.
    private async void OnFindStoreLocationClicked(object sender, EventArgs e)
    {
        try
        {
            if (BindingContext is not NewTaskViewModel vm)
                return;
            // Basic validation to avoid making a geocoding request with empty input
            if (string.IsNullOrWhiteSpace(vm.StoreAddress))
            {
                await DisplayAlert("Store", "Enter a shop name or address first.", "OK");
                return;
            }

            var locations = await Geocoding.Default.GetLocationsAsync(vm.StoreAddress);
            var location = locations?.FirstOrDefault();

            if (location == null)
            {
                await DisplayAlert("Store", "Location not found.", "OK");
                return;
            }

            vm.Latitude = location.Latitude;
            vm.Longitude = location.Longitude;

            await DisplayAlert("Store Found",
                $"Saved location for {vm.StoreAddress}",
                "OK");
        }
        catch (Exception ex)
        {
            // Handles geocoding/API failures without crashing the page
            await DisplayAlert("Geocoding Error", ex.Message, "OK");
        }
    }
   // Clears temporary bottom status messages after a short delay.
    private async Task ClearStatusMessageAfterDelayAsync()
    {
        await Task.Delay(5000);
        _viewModel.StatusMessage = string.Empty;
    }
}