# Play Mode Plus
## Custom Unity Toolbar - Play mode scene selection, play mode settings, build and build presets.

Play Mode Plus extends Unity toolbar with additional tools using the official Unity 6.3 Toolbar API.

## ⚠️ Unity 6.3+ Required
**Version 3.0.0+ requires Unity 6.3 or higher.** This version uses Unity's official toolbar API instead of reflection.

For Unity 2021, 2022, or Unity 6.0-6.2, use [Play Mode Plus v2.0.0](https://github.com/TheDisillusion/Play-Mode-Plus/releases/tag/v2.0.0).

## Features

- **Custom Play Button** - Play the selected scene directly from the toolbar
- **Scene Selector Dropdown** - Choose which scene to launch in play mode
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

### Play Button
Click the custom play button to enter play mode with your selected scene. The button icon changes when in play mode.

### Scene Selection
- Click the scene dropdown to see all scenes in your project
- Select a scene to set it as the play mode start scene
- Use "Add Scene..." to create a new scene
- The last selected scene is remembered between sessions

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

> **Note**: New scenes created via "Add Scene..." are automatically named `NewScene`, `NewScene1`, etc.
> **Note**: Only presets containing `P_` in their name appear in Build Settings dropdown

## Migration from v2.x

See [MIGRATION_GUIDE.md](MIGRATION_GUIDE.md) for details on upgrading from the reflection-based version.

## Technical Details

Built using Unity 6.3's official toolbar APIs:
- `MainToolbarElementAttribute` - Register toolbar elements
- `MainToolbarButton` - Custom buttons
- `MainToolbarDropdown` - Dropdown menus with GenericMenu
- `MainToolbar.Refresh()` - Update toolbar state

## Troubleshooting

**Elements not appearing?**
- Verify Unity 6.3+ is installed
- Reimport the package
- Restart Unity Editor
- Check Console for errors

**Wrong position?**
- Right-click toolbar → Edit Mode
- Drag elements to desired location

## License

MIT
