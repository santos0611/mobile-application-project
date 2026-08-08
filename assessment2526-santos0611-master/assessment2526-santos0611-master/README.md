# Mobile Computing Assessment 25/26
## Sergio camara - 22481885
## Moblie Application Development
## Project Overview
This project is a cross platform task management application developed using **.NET MAUI 9.0**.
The application allows users to create, organise, manage and complete tasks while integrating mobile hardware features and accessibility focused design features.

The Application aims to combine:

- Productivity
- User interaction
- Accessibility
- Real world mobile functionality
- Cross platform development principles

## Technologies Used
NET MAUI
SQLite (local database)
MVVM Architecture
Device APIs: Camera (MediaPicker), Location (Geolocation, Geocoding), Flashlight, Text-to-Speech, Haptic Feedback

- **.NET MAUI 9.0**
- **C#**
- **XAML**
- **SQLite** (local database storage)
- **MVVM Architecture**
- **CommunityToolkit.Maui**
- **Plugin.Maui.Audio**

  ---
  ## NuGet Packages Required
  Install the following packages:

- `sqlite-net-pcl`
- `CommunityToolkit.Maui`
- `Plugin.Maui.Audio`
- ## Prerequisites

To run the project, you need:

- Visual Studio 2022 or 
- .NET MAUI 9.0 workload installed
- Android Emulator or Android physical device


Permissions Required (Android/Android emulator)
Camera
Location 
Flashlight

This is an application that is a cross-platform task management app developed using.NETMAUI 9.0.
It allows users to create, manage, and interact with tasks. With the use of standard features and mobile hardware integrations.

The app focuses on: productivity, user interaction, accessibility, and real-world mobile functionality

Features used in the app:

Task Management features such as:

- Camera
- Location
- Flashlight
- Audio / Media playback

## Main Features

## Task Management

Users can:

- Create tasks
- Edit existing tasks
- Delete tasks
- Mark tasks as completed
- View pending and completed tasks
- Filter tasks:
  - All
  - Completed
  - Pending
  - High Priority
  - Medium Priority
  - Low Priority
- Sort tasks:
  - Due date ascending
  - Due date descending
  - Title A-Z
  - Title Z-A

---
## Task Management Priority System

Tasks support priority levels:

- High
- Medium
- Low

Priority is used for things such as :

- Colour coded task borders
- Sound feedback
- Flashlight alert intensity
- Haptic feedback strength

---

Media integration 
Users can attach images to tasks using the device camera.

Features include:
- Capture task photos using `MediaPicker`
- Store images locally
- Display attached images inside task cards 

## Accessibility Features and WCAG Considerations

The application was designed with accessibility in mind and follows relevant WCAG principles for mobile applications.

### Included Features

### Text-to-Speech
- Tap the speaker button to read tasks aloud.
- Helps visually impaired users.
- Helps users with reading difficulties.

### Large Touch Targets
- Larger buttons reduce misclicks.
- Supports users with motor impairments or weaker hand-eye coordination.

### Large Text Mode
- User-controlled font scaling through settings.
- Supports users with reduced eyesight.

### Dark Mode / Light Mode
- Improves readability depending on environment and time of day.
- Helps reduce eye strain.

### Colour + Text Meaning
- Priority uses both colour and text labels.
- Does not rely on colour alone.

### Haptic Feedback
- Physical confirmation when completing tasks.

### Flashlight Alerts
- Useful for hearing impaired users.
- Helpful when the user is not actively looking at the screen.

### Simple Layout
- Clear labels.
- Logical page flow.
- Readable forms.

### Gesture + Button Alternatives
- Swipe gestures available for completing and deleting tasks.
- Standard buttons are also available.

---

## These accessibility features all align with the four core WCAG principles:

### Perceivable
- Supports dark mode and light mode.
- Large text mode improves readability.
- Good colour contrast between text and backgrounds.
- Priority uses both colour and text labels.

### Operable
- Large touch targets for buttons.
- Swipe gestures are supported, with button alternatives available.
- Clear navigation through tabbed layout.

### Understandable
- Clear labels, placeholders, and simple page structure.
- Consistent task actions (Edit, Complete, Delete).
- Helpful validation and feedback messages.

### Robust
- Cross-platform design using .NET MAUI. 9.0
- Works across Android and Windows environments.
- Uses device APIs with fallback handling where features are unsupported to stop it crashing.

### Validation

- Prevents empty task titles
- Handles invalid location searches
- Clear status messages shown

### Error Handling
handles:

- Camera unavailable
- Permission denied
- Geocoding failures
- Database save errors
- Unsupported device features

The app uses user-friendly messages instead of crashing.

---
## Notifications Was Attempt

Push / local notifications were originally researched and partially tried during development.

However, due to the complex nature of issues I came across with devices/emulators and time constraints, the feature was removed in the end. For a more favourable and more stable in app reminder system using:

- Toast popups
- Sound alerts
- Haptic feedback
- Flashlight alerts

This ensured a more reliable and polished final submission with everyhting working.

## Future Improvements

Possible future upgrades include:

- Fully working local notifications
- Cloud sync
- Recurring tasks
- Categories/tags
- Calendar integration
- Map-based location picker
- Task sharing between users


