# ePicSearch

## Overview

**ePicSearch** is a simple yet creative app developed using .NET MAUI for Android. It lets you create interactive adventures by turning photos into clues for finding a treasure. Whether you want to craft a treasure hunt or just have some fun with friends and family, ePicSearch makes it easy and engaging.

---

## Key Technologies

- **Framework**: .NET MAUI (Multi-platform App UI).
- **Languages**: C# with XAML for UI design.
- **Libraries and Tools**:
  - `CommunityToolkit.Maui`: For enhanced UI components.
  - `Newtonsoft.Json`: For managing data serialization.
  - `Serilog`: For structured logging.
  - `Plugin.Maui.Audio`: For audio features.

---

## Project Structure

### General Structure

- **Project Name**: ePicSearch
- **Namespace**: `ePicSearch`

### Key Files

- **Project File**: `ePicSearch.csproj`
  - Defines the target platform (`net8.0-android`) and project dependencies.
- **App.xaml** and **App.xaml.cs**:
  - Manage global styles, resources, and app lifecycle behavior.
- **AppShell.xaml** and **AppShell.xaml.cs**:
  - Define navigation and app structure.
- **MauiProgram.cs**:
  - Configures services, logging, and dependencies.

### Folder Organization

- **Entities**: Core data models like `AdventureData` and `PhotoInfo`.
- **Services**: App logic, including:
  - `AdventureManager`: Handles adventure creation and management.
  - `PhotoStorageService`: Manages photo storage and retrieval.
  - `DataStorageService`: Handles saving and loading adventures.
- **Views**: UI components:
  - **CameraPage**: Capture photos and create clues.
  - **GamePage**: Play through adventures.
  - **MyAdventuresPage**: Manage saved adventures.
  - **NewAdventurePage**: Start new adventures.
  - **SettingsPage**: Access settings like clearing logs.
  - **TutorialPage**: A step-by-step guide for new users.
- **Behaviors**: Custom visual effects, like `BlurBehavior`.
- **Helpers**: Utility classes for animations and interactions.

### Resources

- **Images**: Custom graphics for UI.
- **Fonts**: Unique fonts for styling.
- **Raw Assets**: Audio files and other media.

---

## How It Works

### Creating Adventures

1. Take a photo of your "treasure" hiding spot - it will be your first clue.
2. Recieve an unlocking code, write it sown and hide it in other place - its your second clue.
3. Take a photo of the general location where you hid your clue.
4. Recieve a code, write it down, and keep going, build a trail of hints to the treasure.

### Playing Adventures

1. Locate your adventure.
2. First photo in the trail will be unlocked - find the location on the photo and search for the code paper.
3. Once found - it unlocks the second photo.
4. Continue untill you find the treasure!

### Logs

- Saved to `logs.txt` in the app's data directory.
- Automatically managed to prevent excessive file size.

---
## Installation

1. Clone the repository:
   
   git clone https://github.com/MikeTat1988/epicSearch.git

2. Open the project in Visual Studio.
3. Build and run the app on an Android device or emulator.
4. You can extract the last log file from the emulated device by running the Logs/PullLogs.exe

## Future Improvements

1. Make an auto naming feature for the new advenures.
2. Introduce an option for a clue if a player struggles finding the code paper.
3. Allow sharing adventures with other users.



   
