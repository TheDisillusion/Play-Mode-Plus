using System.Linq;
using UnityEditor;
using UnityEditor.Presets;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayModePlus.Editor
{
    public static class PlayModePlusToolbarElements
    {
        private const string PlayButtonPath = "Play Mode Plus/Play Button";
        private const string SceneDropdownPath = "Play Mode Plus/Scene Selector";
        private const string TimeScalePath = "Play Mode Plus/Time Scale";
        private const string PlayModeSettingsPath = "Play Mode Plus/Play Mode Settings";
        private const string BuildSettingsPath = "Play Mode Plus/Build Settings";
        private const string BuildButtonPath = "Play Mode Plus/Build Button";
        private const string SelectedScenePrefsKey = SceneDropdownPath + "_SelectedScene";
        private const string ScenesFolderPath = "Assets/Scenes/";
        
        private const float MinTimeScale = 0f;
        private const float MaxTimeScale = 2f;
        private const float TimeScaleUnit = 1f / 100f;

        private static string _selectedPlayModeSetting = "Default (Reload Domain, Reload Scene)";
        private static string _selectedBuildPreset;
        private static SceneAsset _selectedScene;

        static PlayModePlusToolbarElements()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            InitializeSelectedScene();
        }
        
        private static void InitializeSelectedScene()
        {
            var lastScenePath = PlayerPrefs.GetString(SelectedScenePrefsKey, null);
            if (!string.IsNullOrEmpty(lastScenePath))
            {
                _selectedScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(lastScenePath);
            }
            EditorSceneManager.playModeStartScene = _selectedScene;
        }
        
        private static SceneAsset SelectedScene
        {
            get => _selectedScene;
            set
            {
                _selectedScene = value;
                EditorSceneManager.playModeStartScene = value;
                var scenePath = value == null ? null : AssetDatabase.GetAssetPath(value);
                PlayerPrefs.SetString(SelectedScenePrefsKey, scenePath);
            }
        }

        [MainToolbarElement(TimeScalePath, defaultDockPosition = MainToolbarDockPosition.Middle, defaultDockIndex = 0)]
        public static MainToolbarElement CreateTimeScaleSlider()
        {
            var content = new MainToolbarContent("Time Scale", "Time Scale");
            var slider = new MainToolbarSlider(
                content,
                Time.timeScale / TimeScaleUnit,
                MinTimeScale / TimeScaleUnit,
                MaxTimeScale / TimeScaleUnit,
                OnTimeScaleChanged,
                true
            )
            {
                displayed = true
            };
            
            slider.populateContextMenu = menu =>
            {
                menu.ClearItems();
                menu.InsertAction(0, "Reset", _ =>
                {
                    Time.timeScale = 1f;
                    MainToolbar.Refresh(TimeScalePath);
                });
            };
            
            return slider;
        }
        
        [MainToolbarElement(PlayButtonPath, defaultDockPosition = MainToolbarDockPosition.Middle, defaultDockIndex = 1)]
        public static MainToolbarElement CreatePlayButton()
        {
            var playButtonTexture = Resources.Load<Texture2D>("com.disillusion.play-mode-plus/Icons/CustomPlayButton");
            var playStopButtonTexture = Resources.Load<Texture2D>("com.disillusion.play-mode-plus/Icons/CustomPlayStopButton");
            var texture = EditorApplication.isPlaying ? playStopButtonTexture : playButtonTexture;
            var content = new MainToolbarContent(texture, "Play selected scene");
            var button = new MainToolbarButton(content, OnPlayButtonClicked)
            {
                displayed = true
            };
            return button;
        }

        [MainToolbarElement(SceneDropdownPath, defaultDockPosition = MainToolbarDockPosition.Middle, defaultDockIndex = 2)]
        public static MainToolbarElement CreateSceneDropdown()
        {
            var icon = EditorGUIUtility.IconContent("UnityLogo").image as Texture2D;
            var displayText = SelectedScene != null ? SelectedScene.name : "Active Scene";
            var content = new MainToolbarContent(displayText, icon, "Select scene to play");
            var dropdown = new MainToolbarDropdown(content, ShowSceneDropdownMenu)
            {
                displayed = true
            };
            return dropdown;
        }

        [MainToolbarElement(PlayModeSettingsPath, defaultDockPosition = MainToolbarDockPosition.Middle, defaultDockIndex = 3)]
        public static MainToolbarElement CreatePlayModeSettingsDropdown()
        {
            var content = new MainToolbarContent(_selectedPlayModeSetting, null, "Play mode settings");
            var dropdown = new MainToolbarDropdown(content, ShowPlayModeSettingsMenu)
            {
                displayed = true
            };
            return dropdown;
        }

        [MainToolbarElement(BuildButtonPath, defaultDockPosition = MainToolbarDockPosition.Middle, defaultDockIndex = 4)]
        public static MainToolbarElement CreateBuildButton()
        {
            var icon = EditorGUIUtility.IconContent("BuildSettings.Editor.Small").image as Texture2D;
            var content = new MainToolbarContent(icon, "Open Build Window");
            var button = new MainToolbarButton(content, OnBuildButtonClicked)
            {
                displayed = true
            };
            return button;
        }

        [MainToolbarElement(BuildSettingsPath, defaultDockPosition = MainToolbarDockPosition.Middle, defaultDockIndex = 5)]
        public static MainToolbarElement CreateBuildSettingsDropdown()
        {
            var displayText = string.IsNullOrEmpty(_selectedBuildPreset) ? "Build Preset" : _selectedBuildPreset;
            var content = new MainToolbarContent(displayText, null, "Select build preset");
            var dropdown = new MainToolbarDropdown(content, ShowBuildSettingsMenu)
            {
                displayed = true
            };
            return dropdown;
        }

        private static void OnPlayButtonClicked()
        {
            if (!EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = true;
            }
            else
            {
                EditorApplication.isPlaying = false;
            }
        }
        
        private static void OnTimeScaleChanged(float newValue)
        {
            Time.timeScale = newValue * TimeScaleUnit;
        }

        private static void ShowSceneDropdownMenu(Rect dropDownRect)
        {
            var menu = new GenericMenu();
            
            menu.AddItem(new GUIContent("Active Scene"), SelectedScene == null, () =>
            {
                SelectedScene = null;
                MainToolbar.Refresh(SceneDropdownPath);
            });
            
            menu.AddSeparator("");
            
            var scenePaths = AssetDatabase.FindAssets("t:SceneAsset", new[] { "Assets" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .ToArray();

            // Scenes in the "Scenes" folder should always appear at the top
            foreach (var scenePath in scenePaths.Where(p => p.StartsWith(ScenesFolderPath)))
                AddSceneMenuItem(menu, scenePath);

            menu.AddSeparator("");
            foreach (var scenePath in scenePaths.Where(p => !p.StartsWith(ScenesFolderPath)))
                AddSceneMenuItem(menu, scenePath);

            menu.DropDown(dropDownRect);
        }
        
        private static void AddSceneMenuItem(GenericMenu menu, string scenePath)
        {
            var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            if (scene == null) return;
            
            var itemName = ScenePathToMenuPath(scenePath);
            var isSelected = scene == SelectedScene;
            menu.AddItem(new GUIContent(itemName), isSelected, () =>
            {
                SelectedScene = scene;
                MainToolbar.Refresh(SceneDropdownPath);
            });
        }
        
        private static string ScenePathToMenuPath(string scenePath)
        {
            var withoutExtension = scenePath[..^6];
            return RemovePrefix(withoutExtension, ScenesFolderPath, "Assets/", "Packages/");

            static string RemovePrefix(string input, params string[] prefixes)
            {
                if (string.IsNullOrEmpty(input) || prefixes == null || prefixes.Length == 0)
                    return input;

                foreach (var prefix in prefixes)
                {
                    if (!string.IsNullOrEmpty(prefix) && input.StartsWith(prefix))
                        return input[prefix.Length..];
                }

                return input;
            }
        }

        private static void ShowPlayModeSettingsMenu(Rect dropDownRect)
        {
            var menu = new GenericMenu();
            var settings = new[]
            {
                "Default (Reload Domain, Reload Scene)",
                "Disable Reload Domain",
                "Disable Reload Scene",
                "Disable All"
            };

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
            var presetGuids = AssetDatabase.FindAssets("t:Preset");
            var buildPresets = presetGuids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<Preset>)
                .Where(p => p != null && p.name.Contains("P_"))
                .ToArray();

            if (buildPresets.Length == 0)
            {
                menu.AddDisabledItem(new GUIContent("No Build Presets Found"));
            }
            else
            {
                foreach (var preset in buildPresets)
                {
                    var isSelected = preset.name == _selectedBuildPreset;
                    menu.AddItem(new GUIContent(preset.name), isSelected, () =>
                    {
                        _selectedBuildPreset = preset.name;
                        ApplyBuildPreset(preset);
                        MainToolbar.Refresh(BuildSettingsPath);
                    });
                }
            }

            menu.DropDown(dropDownRect);
        }

        private static void ApplyBuildPreset(Preset preset)
        {
            var projectSettings = Resources.FindObjectsOfTypeAll<PlayerSettings>();
            foreach (var settings in projectSettings)
            {
                preset.ApplyTo(settings);
            }
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
            EditorWindow.GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            MainToolbar.Refresh(PlayButtonPath);
        }
    }
}
