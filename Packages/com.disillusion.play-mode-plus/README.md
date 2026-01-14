# Play Mode Plus
## Custom Unity Toolbar - Time scale control, play mode scene selection, play mode settings, and build presets.

Play Mode Plus extends Unity toolbar with additional tools using the official Unity 6.3 Toolbar API.

## ⚠️ Unity 6.3+ Required
**Version 3.0.0+ requires Unity 6.3 or higher.** This version uses Unity's official toolbar API instead of reflection.

For Unity 2021, 2022, or Unity 6.0-6.2, use [Play Mode Plus v2.0.0](https://github.com/TheDisillusion/Play-Mode-Plus/releases/tag/v2.0.0).

## Features

- **Time Scale Slider** - Adjust game speed from 0x to 2x during play mode
- **Scene Selector Dropdown** - Choose which scene to launch when clicking Unity's play button
- **Play Mode Settings** - Configure domain/scene reload options
- **Build Button** - Quick access to build window
- **Build Presets** - Apply PlayerSettings presets from dropdown
- **Fully Customizable** - Hide, show, and reorder toolbar elements

## Installation

1. Open Unity 6.3 or higher
2. Add package via Package Manager:
   - Git URL: `https://github.com/TheDisillusion/Play-Mode-Plus.git`
   - Or download and import manually

## Usage

### Time Scale Slider
- Adjust game speed during play mode (0.0x to 2.0x)
- Right-click slider to reset to 1.0x
- Useful for testing slow-motion or fast-forward scenarios

### Scene Selection
- Click the scene dropdown to select which scene to launch in play mode
- Choose "Active Scene" to play the currently open scene
- Click Unity's native play button to enter play mode with selected scene
- Scenes in `Assets/Scenes/` folder appear first
- Selected scene persists between Unity sessions
- Scene paths are displayed with clean, readable names
- **Note**: Unit tests are unaffected by scene selection

### Play Mode Settings
Configure how Unity enters play mode:
- **Default** - Reload domain and scene (standard Unity behavior)
- **Disable Reload Domain** - Faster iteration, preserves static state
- **Disable Reload Scene** - Keep scene state between play sessions
- **Disable All** - Maximum speed, requires careful initialization

### Build Presets
- Create PlayerSettings presets with `P_` prefix in the name
- Select from dropdown to apply preset
- Click build button to open build window

### Customization
Right-click the toolbar to:
- Hide/show individual elements
- Enter Edit Mode to reorder elements
- Save/load toolbar presets
- Or use Ctrl+Drag (Cmd+Drag on macOS) to reorder

## Naming Conventions

> **Note**: Only presets containing `P_` in their name appear in Build Settings dropdown

## Migration from v2.x

See [MIGRATION_GUIDE.md](MIGRATION_GUIDE.md) for details on upgrading from the reflection-based version.

## Technical Details

Built using Unity 6.3's official toolbar APIs:
- `MainToolbarElementAttribute` - Register toolbar elements
- `MainToolbarButton` - Custom buttons
- `MainToolbarDropdown` - Dropdown menus with GenericMenu
- `MainToolbarSlider` - Time scale control
- `EditorSceneManager.playModeStartScene` - Scene override on play
- `MainToolbar.Refresh()` - Update toolbar state

## Troubleshooting

**Elements not appearing?**
- Right-click the toolbar (three dots menu in top-right)
- Navigate to **Play Mode Plus** section
- Click **Show All** to make all elements visible
- Alternatively, verify Unity 6.3+ is installed and restart Unity Editor

**Wrong position?**
- Right-click toolbar → Edit Mode
- Drag elements to desired location

## License

MIT
