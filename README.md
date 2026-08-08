# Cross-Platform Task Management Application

A cross-platform task management application developed using **.NET MAUI 9.0**.

The application allows users to create, organise, manage and complete tasks while integrating mobile device hardware, local data storage and accessibility-focused design features.

The project was designed around five core areas:

- Productivity
- User interaction
- Accessibility
- Real-world mobile functionality
- Cross-platform development principles

---

## Technologies Used

The application was developed using:

- **.NET MAUI 9.0**
- **C#**
- **XAML**
- **SQLite** for local database storage
- **MVVM architecture**
- **CommunityToolkit.Maui**
- **Plugin.Maui.Audio**

The application also makes use of several native device APIs, including:

- Camera using `MediaPicker`
- Location using `Geolocation`
- Address lookup using `Geocoding`
- Flashlight
- Text-to-Speech
- Haptic feedback
- Audio and media playback

---

## NuGet Packages

The following NuGet packages are required:

- `sqlite-net-pcl`
- `CommunityToolkit.Maui`
- `Plugin.Maui.Audio`

---

## Prerequisites

To run the project, you will need:

- Visual Studio 2022
- .NET MAUI 9.0 workload installed
- Android Emulator or a physical Android device

### Android Permissions

The application requires access to selected device functionality, including:

- Camera
- Location
- Flashlight

Permission handling is implemented to prevent unsupported or unavailable device features from causing application crashes.

---

# Main Features

## Task Management

The core functionality of the application allows users to:

- Create tasks
- Edit existing tasks
- Delete tasks
- Mark tasks as completed
- View pending tasks
- View completed tasks

Users can also filter tasks by:

- All
- Completed
- Pending
- High Priority
- Medium Priority
- Low Priority

Tasks can be sorted by:

- Due date ascending
- Due date descending
- Title A-Z
- Title Z-A

---

## Task Priority System

Tasks can be assigned one of three priority levels:

- High
- Medium
- Low

The priority system is used throughout the application to provide visual and physical feedback.

Priority levels can affect:

- Colour-coded task borders
- Sound feedback
- Flashlight alert intensity
- Haptic feedback strength

This provides users with multiple ways of identifying the importance of a task.

---

## Media Integration

Users can attach images directly to tasks using the device camera.

Features include:

- Capturing task photographs using `MediaPicker`
- Storing images locally
- Displaying attached images within task cards

This allows users to add visual context to individual tasks.

---

## Location Features

The application integrates device location functionality using:

- `Geolocation`
- `Geocoding`

Location functionality can be used alongside task information while also handling scenarios where location access is unavailable or permission is denied.

Invalid location searches and geocoding failures are handled through user-friendly feedback rather than application crashes.

---

## Audio and Device Feedback

The application integrates multiple forms of device feedback to improve interaction with tasks.

These include:

- Audio playback
- Text-to-Speech
- Haptic feedback
- Flashlight alerts
- Toast messages

These features provide additional visual, physical and audio feedback depending on the task or action being performed.

---

# Accessibility

Accessibility was an important consideration throughout the application's design.

The interface incorporates features intended to support users with different visual, motor and hearing requirements.

## Text-to-Speech

Users can select the speaker control to have task information read aloud.

This can support:

- Users with visual impairments
- Users with reading difficulties
- Users who prefer audio-based interaction

---

## Large Touch Targets

Buttons and interactive controls use larger touch areas to reduce accidental inputs.

This can improve usability for users with:

- Motor impairments
- Reduced dexterity
- Reduced hand-eye coordination

---

## Large Text Mode

Users can enable larger text through the application settings.

This provides user-controlled font scaling and improves readability for users with reduced eyesight.

---

## Dark Mode and Light Mode

The application supports both:

- Dark mode
- Light mode

This allows users to select a display mode that is more comfortable depending on their environment and personal preference.

It can also help reduce eye strain in different lighting conditions.

---

## Colour and Text-Based Information

Task priority is represented using both:

- Colour
- Text labels

The application therefore does not rely exclusively on colour to communicate meaning.

---

## Haptic Feedback

Haptic feedback provides physical confirmation when users perform actions such as completing tasks.

Feedback strength can also vary depending on task priority.

---

## Flashlight Alerts

The flashlight can be used as an additional alert mechanism.

This may benefit:

- Hearing-impaired users
- Users who are not actively looking at the screen
- Situations where visual device feedback is useful

Flashlight behaviour can also vary according to task priority.

---

## Simple Interface Design

The user interface was designed around:

- Clear labels
- Logical page flow
- Readable forms
- Consistent controls
- Simple navigation

The aim was to minimise unnecessary complexity while maintaining the application's functionality.

---

## Gesture and Button Alternatives

The application supports swipe gestures for actions such as:

- Completing tasks
- Deleting tasks

Equivalent standard buttons are also available.

This provides users with alternative ways of carrying out the same actions rather than relying entirely on gestures.

---

# WCAG Considerations

The accessibility features were developed with the four core WCAG principles in mind.

## Perceivable

The application supports:

- Dark mode
- Light mode
- Large text mode
- Readable text and background contrast
- Priority identification using both colour and text

---

## Operable

The application includes:

- Large touch targets
- Swipe gestures
- Button alternatives to gestures
- Clear tab-based navigation
- Accessible interaction methods

---

## Understandable

The application uses:

- Clear labels
- Descriptive placeholders
- Consistent page layouts
- Consistent task actions
- Clear validation
- User-friendly feedback messages

Core task actions remain consistent throughout the application:

- Edit
- Complete
- Delete

---

## Robust

The application is built using **.NET MAUI 9.0** and follows a cross-platform architecture.

It is designed to operate across supported environments including:

- Android
- Windows

Device APIs include fallback and exception handling where functionality is unsupported.

This reduces the likelihood of unsupported hardware features causing application crashes.

---

# Validation

The application includes input validation and user feedback.

Examples include:

- Preventing empty task titles
- Handling invalid location searches
- Displaying clear status messages
- Preventing invalid task information from being saved

---

# Error Handling

The application includes error handling for several potential issues, including:

- Camera unavailable
- Camera permission denied
- Location permission denied
- Geocoding failures
- Database save errors
- Unsupported device features
- Hardware functionality unavailable

Where possible, the application displays clear user-friendly messages instead of terminating unexpectedly.

---

# Notifications and Reminder System

Push and local notifications were originally researched and partially implemented during development.

However, compatibility issues across physical devices and emulators, combined with development time constraints, meant the notification functionality could not be implemented to the required level of stability.

The feature was therefore removed in favour of a more reliable in-application reminder system.

The final application uses:

- Toast notifications
- Sound alerts
- Haptic feedback
- Flashlight alerts

This decision prioritised application stability and ensured that the implemented functionality operated consistently in the final version.

---

# Architecture

The application follows the **MVVM (Model-View-ViewModel)** architecture.

This provides separation between:

- User interface components
- Application logic
- Data models
- Data access

Using MVVM helps improve maintainability and keeps the application's interface and logic more clearly separated.

SQLite is used for persistent local data storage, allowing task information to remain available between application sessions.

---

# Project Focus

The project demonstrates practical experience in:

- Cross-platform mobile application development
- C# development
- XAML interface development
- MVVM architecture
- Local SQLite databases
- Mobile hardware integration
- Device permission handling
- Accessibility-focused UI design
- Input validation
- Exception handling
- Media integration
- Location services
- User feedback systems

---

# Future Improvements

Potential future improvements include:

- Fully implemented local notifications
- Cloud synchronisation
- Recurring tasks
- Categories and tags
- Calendar integration
- Map-based location picker
- Task sharing between users

These features would extend the application from a locally managed task system into a more complete productivity platform.
