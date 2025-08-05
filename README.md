# Roblox External ESP Tool

**EDUCATIONAL PURPOSE ONLY** - This project is designed for learning about memory reading, Windows API, and DirectX overlay development.

## Features

### ESP (Extra Sensory Perception)
- **Skeleton ESP**: Shows player bone structure
- **Health Bar**: Visual health indicator with percentage
- **Health Number**: Numerical health display
- **Player Name**: Shows player usernames
- **Distance ESP**: Displays distance to players
- **Head Circle**: Circular indicator around player heads
- **Box ESP**: Multiple box types (2D, 3D, Corner boxes)
- **Sonar ESP**: Radar-style detection
- **China Hat**: Traditional hat-style indicator
- **Chams**: Through-wall player highlighting
- **Box Glow**: Glowing bounding boxes

### Technical Features
- **Real-time Process Detection**: Monitors for RobloxPlayerBeta.exe using CMD and Process API
- **Memory Reading**: Uses Windows API (ReadProcessMemory) for safe memory access
- **DirectX Overlay**: Hardware-accelerated rendering using SharpDX
- **World-to-Screen Conversion**: Proper 3D to 2D coordinate transformation
- **GUI Overlay**: Both ESP and GUI render on top of Roblox window
- **Color Customization**: Full color picker for all ESP elements

## Architecture

### Core Components
- `Core/WinAPI.cs` - Windows API wrappers for memory reading
- `Core/MemoryReader.cs` - Memory reading abstraction layer
- `Core/ProcessMonitor.cs` - Process detection and monitoring
- `Core/RobloxOffsets.cs` - Memory offsets for Roblox version b8550645b8834e8a
- `Core/RobloxMemoryScanner.cs` - Roblox-specific memory scanning

### ESP System
- `ESP/ESPManager.cs` - Central ESP coordination
- `ESP/ESPSettings.cs` - Configuration management
- `Overlay/OverlayRenderer.cs` - DirectX rendering engine
- `Overlay/OverlayWindow.cs` - Overlay window management

### GUI
- `MainWindow.xaml` - Main interface (Aimvex theme)
- `Controls/ColorPickerControl.cs` - Custom color picker
- Modern dark theme with purple accents

## Requirements

- .NET 6.0 or higher
- Windows 10/11
- SharpDX libraries for DirectX rendering
- Administrative privileges (for memory reading)

## Building

1. Clone the repository
2. Open in Visual Studio 2022 or JetBrains Rider
3. Restore NuGet packages
4. Build in Release mode
5. Run as Administrator

## Usage

1. **Launch the application** as Administrator
2. **Start Roblox** - The tool will automatically detect when RobloxPlayerBeta.exe is running
3. **Join a game** - ESP will activate once in-game
4. **Configure ESP** - Use the Visuals tab to enable/disable features
5. **Customize colors** - Click color squares to change ESP colors

## Memory Offsets

The tool uses memory offsets for Roblox version `version-b8550645b8834e8a`. These offsets may need updating for newer Roblox versions.

Key offsets include:
- DataModel access paths
- Player/Character structures
- Camera and ViewMatrix
- Health and position data

## Legal Notice

**EDUCATIONAL USE ONLY**

This software is provided for educational purposes to demonstrate:
- Windows API memory reading techniques
- DirectX overlay development
- Process monitoring and detection
- 3D mathematics and coordinate transformation

**Important:**
- This tool does NOT inject code into Roblox
- This tool does NOT modify game memory
- This tool only READS memory for educational purposes
- Use responsibly and in accordance with Roblox Terms of Service

## Technical Details

### Memory Reading Process
1. Detect RobloxPlayerBeta.exe process
2. Open process with PROCESS_VM_READ permissions
3. Locate DataModel using static offsets
4. Traverse object hierarchy to find players
5. Read player positions, health, and names
6. Convert 3D world coordinates to 2D screen coordinates

### Overlay Rendering
1. Create transparent overlay window
2. Position overlay on top of Roblox window
3. Use DirectX for hardware-accelerated rendering
4. Render ESP elements at 60 FPS
5. Handle window focus and positioning

### Safety Features
- Read-only memory access
- Process validation before attachment
- Graceful error handling
- Automatic cleanup on exit

## Disclaimer

This project is for educational purposes only. The developers are not responsible for any misuse of this software. Users should comply with all applicable terms of service and local laws.

## Version Information

- **Roblox Version**: version-b8550645b8834e8a
- **Tool Version**: 1.0.0
- **Framework**: .NET 6.0
- **Graphics**: SharpDX (DirectX 11)