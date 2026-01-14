# Changelog

## [3.0.0] - 2026-01-14

### Breaking Changes
- **Minimum Unity Version**: Now requires Unity 6.3 (6000.3) or higher
- Removed reflection-based toolbar implementation
- Not compatible with Unity 2021, 2022, or Unity 6.0-6.2
- **Removed custom play button** - Now uses Unity's native play button

### Added
- **Time Scale Slider** - Adjust game speed from 0x to 2x during play mode with right-click reset
- Official Unity 6.3 Toolbar API implementation
- `PlayModePlusToolbarElements.cs` - New main implementation using `[MainToolbarElement]` attributes
- **Scene Selector Improvements**:
  - "Active Scene" option to play currently open scene
  - Scenes in `Assets/Scenes/` folder prioritized at top
  - PlayerPrefs persistence for selected scene across sessions
  - Clean scene path display (removes `Assets/`, `Packages/` prefixes)
  - Unity logo icon in dropdown
  - **Smart scene override** - Only applies to native play button, not unit tests
- Full support for Unity 6.3 toolbar customization features:
  - Hide/show elements via context menu
  - Reorder elements with Ctrl+Drag (Cmd+Drag on macOS)
  - Save/load toolbar presets
- Comprehensive documentation:
  - `MIGRATION_GUIDE.md` - Migration instructions from v2.x
  - `IMPLEMENTATION_NOTES.md` - Technical deep-dive
  - `CHANGELOG.md` - This file

### Changed
- Refactored from reflection-based to attribute-based toolbar registration
- Replaced `IMGUIContainer` with `MainToolbarButton`, `MainToolbarDropdown`, and `MainToolbarSlider`
- **Now uses Unity's native play button** instead of custom play button
- Scene override applies conditionally:
  - ✅ Applied when clicking native play button
  - ❌ Skipped during unit test execution (test-safe)
- Scene dropdown uses `EditorSceneManager.playModeStartScene` with test detection
- Toolbar elements reordered: Time Scale, Scene Selector, Play Mode Settings, Build Button, Build Settings
- All elements now use `MainToolbarDockPosition.Middle` for consistent placement
- Updated package description to mention official API
- Updated `package.json` to v3.0.0 and Unity 6000.3 minimum

### Removed
- `CustomUnityToolbarCallback.cs` - Reflection-based toolbar injection (deprecated)
- `CustomUnityToolbar.cs` - Old initialization system (deprecated)
- `PlayModeToolbar.cs` - UIElements-based toolbar (deprecated)
- `PlayModePlus.uss` - UIElements stylesheet (no longer needed)
- `PlayModePlusToolbar.uxml` - UIElements template (no longer needed)
- `PlayModeManager.cs` - Replaced with direct API calls
- `BuildManager.cs` - Functionality inlined into main toolbar class
- **Custom play button** - Now uses Unity's native play button
- `CustomPlayButton.png` and `CustomPlayStopButton.png` - No longer needed
- `third-party-notices.md.txt` - No longer using third-party reflection code

### Fixed
- Toolbar no longer breaks with Unity internal API changes
- Console warnings about unsupported toolbar methods eliminated
- Future-proof implementation using official, supported APIs

### Technical Details
- Uses `UnityEditor.Toolbars` namespace
- Implements `MainToolbarButton` for build button
- Implements `MainToolbarDropdown` with `GenericMenu` for dropdowns
- Implements `MainToolbarSlider` for time scale control
- Calls `MainToolbar.Refresh(path)` for UI updates
- Uses `PlayerPrefs` for scene selection persistence
- Conditional scene override with `EditorSceneManager.playModeStartScene`:
  - Applied on `PlayModeStateChange.ExitingEditMode` if not running tests
  - Cleared on `PlayModeStateChange.EnteredEditMode`
- Test detection via TestRunner assembly and stack trace analysis
- Simplified codebase with ~200 fewer lines of code

---

## [2.0.0] - Previous Release

### Features
- Custom play button with scene selection
- Scene selector dropdown
- Play mode settings dropdown
- Build settings dropdown with presets
- Build button shortcut

### Implementation
- Reflection-based toolbar injection
- UIElements-based custom toolbar
- Compatible with Unity 2021, 2022, and Unity 6.0-6.2

---

## Migration Notes

**Upgrading from v2.x to v3.0.0:**
1. Ensure Unity 6.3+ is installed
2. Update package to v3.0.0
3. Restart Unity Editor
4. Verify toolbar elements appear
5. Old files are automatically removed

**Staying on v2.x:**
- If you need Unity 2021, 2022, or Unity 6.0-6.2 support
- Use Play Mode Plus v2.0.0
- Available at: https://github.com/TheDisillusion/Play-Mode-Plus/releases/tag/v2.0.0
