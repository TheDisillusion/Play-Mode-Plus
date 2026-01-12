# Migration Guide - Unity 6.3 Official Toolbar API

## Overview
Play Mode Plus has been refactored to use Unity 6.3's official toolbar API instead of reflection-based access to internal Unity classes.

## What Changed

### Old Implementation (Unity 2021-6.0)
- Used reflection to access `UnityEditor.Toolbar` internal class
- Required `IMGUIContainer` workarounds
- Injected custom UI elements into toolbar zones via reflection
- Brittle and broke with Unity internal changes

### New Implementation (Unity 6.3+)
- Uses official `MainToolbarElementAttribute` API
- Declarative attribute-based registration
- No reflection required
- Future-proof and officially supported
- Cleaner, more maintainable code

## Key API Changes

### Before (Reflection-based)
```csharp
[InitializeOnLoad]
public static class CustomUnityToolbarCallback
{
    private static readonly Type MToolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
    // ... reflection code to access internal toolbar
}
```

### After (Official API)
```csharp
[MainToolbarElement("Play Mode Plus/Play Button", defaultDockPosition = MainToolbarDockPosition.Right)]
public static MainToolbarElement CreatePlayButton()
{
    var content = new MainToolbarContent(icon, "Play selected scene");
    return new MainToolbarButton(content, OnPlayButtonClicked);
}
```

## New Features

### Toolbar Customization
Users can now:
- Hide/show toolbar elements via right-click context menu
- Reorder elements by Ctrl+Drag (Cmd+Drag on macOS)
- Save/load toolbar presets
- All standard Unity 6.3 toolbar customization features

### Refresh Mechanism
Elements automatically refresh when needed:
```csharp
MainToolbar.Refresh(elementPath);
```

## Files Changed

### Removed/Deprecated
- `CustomUnityToolbarCallback.cs` - No longer needed (reflection-based)
- `CustomUnityToolbar.cs` - No longer needed (old initialization)
- `PlayModeToolbar.cs` - Replaced by new implementation

### New Files
- `PlayModePlusToolbarElements.cs` - New official API implementation

### Unchanged
- `PlayModeManager.cs` - Core logic remains the same
- `BuildManager.cs` - Core logic remains the same

## Compatibility

- **Minimum Unity Version**: Unity 6.3 (6000.3)
- **Previous Versions**: Not compatible with Unity 2021, 2022, or Unity 6.0-6.2
- If you need support for older Unity versions, use Play Mode Plus v2.0.0 or earlier

## Testing Checklist

- [ ] Play button appears in toolbar
- [ ] Scene dropdown shows all scenes
- [ ] Selecting a scene updates the dropdown
- [ ] Play button starts play mode with selected scene
- [ ] Play mode settings dropdown works
- [ ] Build settings dropdown shows presets
- [ ] Build button opens build window
- [ ] Toolbar elements can be hidden/shown
- [ ] Toolbar elements can be reordered
- [ ] Play button icon changes when entering play mode

## Troubleshooting

### Elements Not Appearing
1. Check Unity version is 6.3 or higher
2. Reimport the package
3. Restart Unity Editor
4. Check Console for errors

### Elements in Wrong Position
- Right-click toolbar → Edit Mode
- Drag elements to desired position
- Or use Ctrl+Drag (Cmd+Drag on macOS)

### Dropdown Not Updating
- Ensure `MainToolbar.Refresh(path)` is called after state changes
- Check element path matches exactly

## Migration Steps for Developers

If you forked or modified the old code:

1. Remove reflection-based code
2. Create static methods with `[MainToolbarElement]` attribute
3. Return `MainToolbarButton` or `MainToolbarDropdown`
4. Use `GenericMenu` for dropdown menus
5. Call `MainToolbar.Refresh(path)` to update UI

## Resources

- [Unity Toolbar Documentation](https://docs.unity3d.com/Manual/Toolbar.html)
- [MainToolbarElementAttribute API](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Toolbars.MainToolbarElementAttribute.html)
- [MainToolbarDropdown Examples](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Toolbars.MainToolbarDropdown.html)
