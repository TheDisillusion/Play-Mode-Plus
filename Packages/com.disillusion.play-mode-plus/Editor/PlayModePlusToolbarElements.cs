using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;

namespace PlayModePlus.Editor
{
    public static class PlayModePlusToolbarElements
    {
        private const string PlayButtonPath = "Play Mode Plus/Play Button";
        private const string SceneDropdownPath = "Play Mode Plus/Scene Selector";
        private const string PlayModeSettingsPath = "Play Mode Plus/Play Mode Settings";
        private const string BuildSettingsPath = "Play Mode Plus/Build Settings";
        private const string BuildButtonPath = "Play Mode Plus/Build Button";

        private static PlayModeManager _playModeManager;
        private static BuildManager _buildManager;
        private static string _selectedScene;
        private static string _selectedPlayModeSetting = "Default (Reload Domain, Reload Scene)";
        private static string _selectedBuildPreset;

        private static Texture2D _playButtonTexture;
        private static Texture2D _playStopButtonTexture;

        static PlayModePlusToolbarElements()
        {
            _playModeManager = new PlayModeManager();
            _buildManager = new BuildManager();

            _playButtonTexture = Resources.Load<Texture2D>("com.disillusion.play-mode-plus/Icons/CustomPlayButton");
            _playStopButtonTexture = Resources.Load<Texture2D>("com.disillusion.play-mode-plus/Icons/CustomPlayStopButton");

            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            PlayModeManager.EditorStartScene();

            var lastScene = _playModeManager.LastScene;
            _selectedScene = lastScene != null ? lastScene.name : null;

            if (string.IsNullOrEmpty(_selectedScene))
            {
                var firstScene = _playModeManager.ScenesInProject?.FirstOrDefault();
                if (firstScene != null)
                {
                    _selectedScene = firstScene.name;
                    _playModeManager.SelectedScene = firstScene;
                }
            }
            else
            {
                _playModeManager.SelectedScene = _playModeManager.ScenesInProject?.FirstOrDefault(s => s.name == _selectedScene);
            }
        }

        [MainToolbarElement(PlayButtonPath, defaultDockPosition = MainToolbarDockPosition.Right, defaultDockIndex = 0)]
        public static MainToolbarElement CreatePlayButton()
        {
            var texture = EditorApplication.isPlaying ? _playStopButtonTexture : _playButtonTexture;
            var content = new MainToolbarContent(texture, "Play selected scene");
            var button = new MainToolbarButton(content, OnPlayButtonClicked);
            button.displayed = true;
            return button;
        }

        [MainToolbarElement(SceneDropdownPath, defaultDockPosition = MainToolbarDockPosition.Right, defaultDockIndex = 1)]
        public static MainToolbarElement CreateSceneDropdown()
        {
            var displayText = string.IsNullOrEmpty(_selectedScene) ? "Select Scene" : _selectedScene;
            var content = new MainToolbarContent(displayText, null, "Select scene to play");
            var dropdown = new MainToolbarDropdown(content, ShowSceneDropdownMenu);
            dropdown.displayed = true;
            return dropdown;
        }

        [MainToolbarElement(PlayModeSettingsPath, defaultDockPosition = MainToolbarDockPosition.Right, defaultDockIndex = 2)]
        public static MainToolbarElement CreatePlayModeSettingsDropdown()
        {
            var content = new MainToolbarContent(_selectedPlayModeSetting, null, "Play mode settings");
            var dropdown = new MainToolbarDropdown(content, ShowPlayModeSettingsMenu);
            dropdown.displayed = true;
            return dropdown;
        }

        [MainToolbarElement(BuildButtonPath, defaultDockPosition = MainToolbarDockPosition.Right, defaultDockIndex = 3)]
        public static MainToolbarElement CreateBuildButton()
        {
            var icon = EditorGUIUtility.IconContent("BuildSettings.Editor.Small").image as Texture2D;
            var content = new MainToolbarContent(icon, "Open Build Window");
            var button = new MainToolbarButton(content, OnBuildButtonClicked);
            button.displayed = true;
            return button;
        }

        [MainToolbarElement(BuildSettingsPath, defaultDockPosition = MainToolbarDockPosition.Right, defaultDockIndex = 4)]
        public static MainToolbarElement CreateBuildSettingsDropdown()
        {
            var displayText = string.IsNullOrEmpty(_selectedBuildPreset) ? "Build Preset" : _selectedBuildPreset;
            var content = new MainToolbarContent(displayText, null, "Select build preset");
            var dropdown = new MainToolbarDropdown(content, ShowBuildSettingsMenu);
            dropdown.displayed = true;
            return dropdown;
        }

        private static void OnPlayButtonClicked()
        {
            _playModeManager.PlayScene();
            _playModeManager.LastScene = _playModeManager.SelectedScene;
        }

        private static void ShowSceneDropdownMenu(Rect dropDownRect)
        {
            var menu = new GenericMenu();
            var sceneList = _playModeManager.GenerateSceneList();

            foreach (var sceneName in sceneList)
            {
                if (sceneName == "Add Scene...")
                {
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent(sceneName), false, () =>
                    {
                        _playModeManager.AddScene();
                        MainToolbar.Refresh(SceneDropdownPath);
                    });
                }
                else
                {
                    var isSelected = sceneName == _selectedScene;
                    menu.AddItem(new GUIContent(sceneName), isSelected, () =>
                    {
                        _selectedScene = sceneName;
                        _playModeManager.SelectedScene = _playModeManager.ScenesInProject.FirstOrDefault(s => s.name == sceneName);
                        _playModeManager.LastScene = _playModeManager.SelectedScene;
                        MainToolbar.Refresh(SceneDropdownPath);
                    });
                }
            }

            menu.DropDown(dropDownRect);
        }

        private static void ShowPlayModeSettingsMenu(Rect dropDownRect)
        {
            var menu = new GenericMenu();
            var settings = _playModeManager.GeneratePlayModeSettingsList();

            foreach (var setting in settings)
            {
                var isSelected = setting == _selectedPlayModeSetting;
                menu.AddItem(new GUIContent(setting), isSelected, () =>
                {
                    _selectedPlayModeSetting = setting;
                    ApplyPlayModeSetting(setting);
                    MainToolbar.Refresh(PlayModeSettingsPath);
                });
            }

            menu.DropDown(dropDownRect);
        }

        private static void ShowBuildSettingsMenu(Rect dropDownRect)
        {
            var menu = new GenericMenu();
            var presets = _buildManager.GenerateBuildSettingsList();

            if (presets.Count == 0)
            {
                menu.AddDisabledItem(new GUIContent("No Build Presets Found"));
            }
            else
            {
                foreach (var preset in presets)
                {
                    var isSelected = preset == _selectedBuildPreset;
                    menu.AddItem(new GUIContent(preset), isSelected, () =>
                    {
                        _selectedBuildPreset = preset;
                        _buildManager.SelectedBuildPreset = _buildManager.PlayerSettingsPresetsInProject.FirstOrDefault(p => p.name == preset);
                        BuildManager.ApplyPreset(_buildManager.SelectedBuildPreset);
                        MainToolbar.Refresh(BuildSettingsPath);
                    });
                }
            }

            menu.DropDown(dropDownRect);
        }

        private static void ApplyPlayModeSetting(string setting)
        {
            var editorSettings = new SerializedObject(
                AssetDatabase.LoadAssetAtPath("ProjectSettings/EditorSettings.asset", typeof(Object)));
            var enterPlayModeOptionsEnabled = editorSettings.FindProperty("m_EnterPlayModeOptionsEnabled");
            var enterPlayModeOptions = editorSettings.FindProperty("m_EnterPlayModeOptions");

            switch (setting)
            {
                case "Default (Reload Domain, Reload Scene)":
                    enterPlayModeOptionsEnabled.boolValue = false;
                    enterPlayModeOptions.intValue = (int)EnterPlayModeOptions.None;
                    break;
                case "Disable Reload Domain":
                    enterPlayModeOptionsEnabled.boolValue = true;
                    enterPlayModeOptions.intValue = (int)EnterPlayModeOptions.DisableDomainReload;
                    break;
                case "Disable Reload Scene":
                    enterPlayModeOptionsEnabled.boolValue = true;
                    enterPlayModeOptions.intValue = (int)EnterPlayModeOptions.DisableSceneReload;
                    break;
                case "Disable All":
                    enterPlayModeOptionsEnabled.boolValue = true;
                    enterPlayModeOptions.intValue = (int)EnterPlayModeOptions.DisableDomainReload | (int)EnterPlayModeOptions.DisableSceneReload;
                    break;
                default:
                    enterPlayModeOptionsEnabled.boolValue = false;
                    enterPlayModeOptions.intValue = (int)EnterPlayModeOptions.None;
                    break;
            }

            editorSettings.ApplyModifiedProperties();
            EditorUtility.SetDirty(editorSettings.targetObject);
            AssetDatabase.SaveAssets();
        }

        private static void OnBuildButtonClicked()
        {
            _buildManager.OpenBuildWindow();
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            MainToolbar.Refresh(PlayButtonPath);
        }
    }
}
